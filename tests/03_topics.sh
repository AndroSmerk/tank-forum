#!/bin/bash
# ============================================
# ТЕСТ: Темы (Topics)
# ОПИСАНИЕ: Проверка CRUD тем с пагинацией, фильтрацией, правами
# ЭНДПОИНТЫ: GET/POST/PUT/DELETE /api/topics
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: 200 для чтения, 201 для создания, 401 без токена, 204 для удаления (админ)
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

TOKEN=$(cat /tmp/user_token.txt 2>/dev/null)
ADMIN_TOKEN=$(cat /tmp/admin_token.txt 2>/dev/null)

echo "============================================"
echo "ТЕСТ 03: Темы"
echo "============================================"

# --- Тест 3.1: Список тем с пагинацией ---
echo ""
echo "=== Тест 3.1: GET /api/topics (пагинация) ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/topics?page=1&pageSize=20")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  TOTAL=$(echo "$BODY" | jq -r '.totalCount // 0')
  echo "✅ PASS: Список тем получен (HTTP $HTTP_CODE, всего тем: $TOTAL)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 3.2: Фильтр по категории ---
echo ""
echo "=== Тест 3.2: GET /api/topics?categoryId=1 ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/topics?categoryId=1")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Фильтр по категории работает (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 3.3: Конкретная тема ---
echo ""
echo "=== Тест 3.3: GET /api/topics/1 ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/topics/1")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  TITLE=$(echo "$BODY" | jq -r '.title // "unknown"')
  echo "✅ PASS: Тема #1 получена (HTTP $HTTP_CODE, title: $TITLE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 3.4: Несуществующая тема ---
echo ""
echo "=== Тест 3.4: GET /api/topics/99999 ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/topics/99999")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "404" ]; then
  echo "✅ PASS: Несуществующая тема возвращает 404 (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 404, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Тест 3.5: Создание темы (авторизованный) ---
echo ""
echo "=== Тест 3.5: POST /api/topics (с токеном) ==="
if [ -n "$TOKEN" ]; then
  # Получаем sectionId
  SECTION_ID=$(curl -s "$BASE_URL/api/sections" | jq -r '.[0].id // 1')
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/topics" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"sectionId\":$SECTION_ID,\"title\":\"Тестовая тема $(date +%s)\",\"content\":\"Содержание тестовой темы\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)
  BODY=$(echo "$RESPONSE" | head -n -1)

  if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    TOPIC_ID=$(echo "$BODY" | jq -r '.id // empty')
    echo "✅ PASS: Тема создана (HTTP $HTTP_CODE, id: $TOPIC_ID)"
    PASSED=$((PASSED + 1))
    echo "$TOPIC_ID" > /tmp/created_topic_id.txt
  else
    echo "❌ FAIL: Создание темы вернуло HTTP $HTTP_CODE"
    echo "   Body: $BODY"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Токен пользователя не найден"
  FAILED=$((FAILED + 1))
fi

# --- Тест 3.6: Создание темы без токена ---
echo ""
echo "=== Тест 3.6: POST /api/topics (без токена) ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/topics" \
  -H "Content-Type: application/json" \
  -d '{"sectionId":1,"title":"Неавторизованная тема","content":"Тест"}')

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "401" ]; then
  echo "✅ PASS: Без токена — 401 (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 401, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Тест 3.7: Редактирование темы ---
echo ""
echo "=== Тест 3.7: PUT /api/topics/{id} (своя тема) ==="
CREATED_TOPIC_ID=$(cat /tmp/created_topic_id.txt 2>/dev/null)
if [ -n "$TOKEN" ] && [ -n "$CREATED_TOPIC_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/topics/$CREATED_TOPIC_ID" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"title\":\"Обновлённая тема $(date +%s)\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Тема обновлена (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID созданной темы или токена"
fi

# --- Тест 3.8: Удаление темы (админ) ---
echo ""
echo "=== Тест 3.8: DELETE /api/topics/{id} (админ) ==="
if [ -n "$ADMIN_TOKEN" ] && [ -n "$CREATED_TOPIC_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X DELETE "$BASE_URL/api/topics/$CREATED_TOPIC_ID" \
    -H "Authorization: Bearer $ADMIN_TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "204" ]; then
    echo "✅ PASS: Тема удалена админом (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 204, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID темы или админ-токена"
fi

# --- Итог ---
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 03:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
