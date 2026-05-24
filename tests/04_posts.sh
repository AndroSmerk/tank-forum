#!/bin/bash
# ============================================
# ТЕСТ: Посты (Posts)
# ОПИСАНИЕ: Проверка CRUD постов, лайков, прав доступа
# ЭНДПОИНТЫ: GET/POST/PUT/DELETE /api/posts, POST /api/posts/{id}/like
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: Корректные HTTP статусы
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

TOKEN=$(cat /tmp/user_token.txt 2>/dev/null)
ADMIN_TOKEN=$(cat /tmp/admin_token.txt 2>/dev/null)

echo "============================================"
echo "ТЕСТ 04: Посты"
echo "============================================"

# --- Тест 4.1: Список постов темы ---
echo ""
echo "=== Тест 4.1: GET /api/posts?topicId=1 ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/posts?topicId=1")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Список постов получен (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 4.2: Создание поста (авторизованный) ---
echo ""
echo "=== Тест 4.2: POST /api/posts (с токеном) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/posts" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"topicId\":1,\"content\":\"Тестовый пост $(date +%s)\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)
  BODY=$(echo "$RESPONSE" | head -n -1)

  if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    POST_ID=$(echo "$BODY" | jq -r '.id // empty')
    echo "✅ PASS: Пост создан (HTTP $HTTP_CODE, id: $POST_ID)"
    PASSED=$((PASSED + 1))
    echo "$POST_ID" > /tmp/created_post_id.txt
  else
    echo "❌ FAIL: Создание поста вернуло HTTP $HTTP_CODE"
    echo "   Body: $BODY"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Токен пользователя не найден"
  FAILED=$((FAILED + 1))
fi

# --- Тест 4.3: Создание поста без токена ---
echo ""
echo "=== Тест 4.3: POST /api/posts (без токена) ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/posts" \
  -H "Content-Type: application/json" \
  -d '{"topicId":1,"content":"Неавторизованный пост"}')

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "401" ]; then
  echo "✅ PASS: Без токена — 401 (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 401, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Тест 4.4: Создание поста с пустым содержимым ---
echo ""
echo "=== Тест 4.4: POST /api/posts (пустое content) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/posts" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d '{"topicId":1,"content":""}')

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "400" ]; then
    echo "✅ PASS: Пустое содержимое — 400 (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 400, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет токена"
fi

# --- Тест 4.5: Редактирование своего поста ---
echo ""
echo "=== Тест 4.5: PUT /api/posts/{id} (свой пост) ==="
CREATED_POST_ID=$(cat /tmp/created_post_id.txt 2>/dev/null)
if [ -n "$TOKEN" ] && [ -n "$CREATED_POST_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/posts/$CREATED_POST_ID" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"content\":\"Обновлённый пост $(date +%s)\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Пост обновлён (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID поста или токена"
fi

# --- Тест 4.6: Лайк поста ---
echo ""
echo "=== Тест 4.6: POST /api/posts/{id}/like ==="
if [ -n "$TOKEN" ] && [ -n "$CREATED_POST_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/posts/$CREATED_POST_ID/like" \
    -H "Authorization: Bearer $TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Лайк поставлен (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Лайк вернул HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID поста или токена"
fi

# --- Тест 4.7: Удаление поста (админ) ---
echo ""
echo "=== Тест 4.7: DELETE /api/posts/{id} (админ) ==="
if [ -n "$ADMIN_TOKEN" ] && [ -n "$CREATED_POST_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/api/posts/$CREATED_POST_ID" \
    -H "Authorization: Bearer $ADMIN_TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "204" ]; then
    echo "✅ PASS: Пост удалён админом (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 204, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID поста или админ-токена"
fi

# --- Итог ---
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 04:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
