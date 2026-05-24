#!/bin/bash
# ============================================
# ТЕСТ: Аутентификация и регистрация
# ОПИСАНИЕ: Проверка регистрации, входа, забытого пароля
# ЭНДПОИНТЫ: POST /api/auth/register, /api/auth/login, /api/auth/forgot-password, /api/auth/reset-password
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: Корректные HTTP статусы и JWT токен
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

echo "============================================"
echo "ТЕСТ 01: Аутентификация и регистрация"
echo "============================================"

# --- Тест 1.1: Регистрация нового пользователя ---
echo ""
echo "=== Тест 1.1: Регистрация нового пользователя ==="
RANDOM_SUFFIX=$RANDOM
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"TestUser_$RANDOM_SUFFIX\",\"email\":\"test_$RANDOM_SUFFIX@example.com\",\"password\":\"Test123!\"}")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
  echo "✅ PASS: Регистрация успешна (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Регистрация вернула HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 1.2: Регистрация с существующим email ---
echo ""
echo "=== Тест 1.2: Регистрация с существующим email ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"TestUser2_$RANDOM_SUFFIX\",\"email\":\"test_$RANDOM_SUFFIX@example.com\",\"password\":\"Test123!\"}")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "409" ]; then
  echo "✅ PASS: Конфликт при повторном email (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 409, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 1.3: Вход с верными данными (Командир) ---
echo ""
echo "=== Тест 1.3: Вход с верными данными (Командир) ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"Командир","password":"password"}')

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  TOKEN=$(echo "$BODY" | jq -r '.token // empty')
  USERNAME=$(echo "$BODY" | jq -r '.username // empty')
  ROLE=$(echo "$BODY" | jq -r '.role // empty')
  if [ -n "$TOKEN" ] && [ "$TOKEN" != "null" ]; then
    echo "✅ PASS: Вход успешен (HTTP $HTTP_CODE, username: $USERNAME, role: $ROLE)"
    PASSED=$((PASSED + 1))
    # Сохраняем токен для следующих тестов
    echo "$TOKEN" > /tmp/admin_token.txt
  else
    echo "❌ FAIL: Токен не получен"
    echo "   Body: $BODY"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Вход вернул HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 1.4: Вход с неверным паролем ---
echo ""
echo "=== Тест 1.4: Вход с неверным паролем ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"Командир","password":"wrong_password"}')

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "401" ]; then
  echo "✅ PASS: Неверный пароль отклонён (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 401, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 1.5: Вход обычного пользователя ---
echo ""
echo "=== Тест 1.5: Вход обычного пользователя (Полковник_Медведь) ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{"usernameOrEmail":"Полковник_Медведь","password":"password"}')

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  TOKEN=$(echo "$BODY" | jq -r '.token // empty')
  echo "✅ PASS: Вход обычного пользователя успешен (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
  echo "$TOKEN" > /tmp/user_token.txt
else
  echo "❌ FAIL: Вход вернул HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 1.6: Восстановление пароля ---
echo ""
echo "=== Тест 1.6: Запрос восстановления пароля ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/forgot-password" \
  -H "Content-Type: application/json" \
  -d '{"email":"test_'"$RANDOM_SUFFIX"'@example.com"}')

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Запрос восстановления принят (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Итог ---
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 01:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
