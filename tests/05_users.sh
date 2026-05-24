#!/bin/bash
# ============================================
# ТЕСТ: Пользователи
# ОПИСАНИЕ: Проверка профилей, поиска, онлайна, обновления
# ЭНДПОИНТЫ: GET/PUT /api/users, POST /api/users/heartbeat
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: Корректные HTTP статусы
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

TOKEN=$(cat /tmp/user_token.txt 2>/dev/null)
ADMIN_TOKEN=$(cat /tmp/admin_token.txt 2>/dev/null)

echo "============================================"
echo "ТЕСТ 05: Пользователи"
echo "============================================"

# --- Тест 5.1: Публичный профиль ---
echo ""
echo "=== Тест 5.1: GET /api/users/1 ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/users/1")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  USERNAME=$(echo "$BODY" | jq -r '.username // "unknown"')
  echo "✅ PASS: Профиль пользователя #1 получен (HTTP $HTTP_CODE, username: $USERNAME)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 5.2: Статистика пользователя ---
echo ""
echo "=== Тест 5.2: GET /api/users/1/stats ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/users/1/stats")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Статистика получена (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 5.3: Поиск пользователей ---
echo ""
echo "=== Тест 5.3: GET /api/users/search?q=Командир ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/users/search?q=%D0%9A%D0%BE%D0%BC%D0%B0%D0%BD%D0%B4%D0%B8%D1%80")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Поиск работает (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 5.4: Список онлайн ---
echo ""
echo "=== Тест 5.4: GET /api/users/online ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/users/online")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Список онлайн получен (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Тест 5.5: Heartbeat (обновление активности) ---
echo ""
echo "=== Тест 5.5: POST /api/users/heartbeat ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/users/heartbeat" \
    -H "Authorization: Bearer $TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Heartbeat принят (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Heartbeat вернул HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет токена"
fi

# --- Тест 5.6: Обновление профиля ---
echo ""
echo "=== Тест 5.6: PUT /api/users/{id} (обновление профиля) ==="
if [ -n "$ADMIN_TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/users/1" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $ADMIN_TOKEN" \
    -d '{"vocation":"Танкист","city":"Москва","quote":"Броня крепка!"}')

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Профиль обновлён (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Обновление вернуло HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет админ-токена"
fi

# --- Тест 5.7: Кланы пользователя ---
echo ""
echo "=== Тест 5.7: GET /api/users/1/clans ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/users/1/clans")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Список кланов пользователя получен (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Итог ---
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 05:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
