#!/bin/bash
# ============================================
# ТЕСТ: E2E сценарий пользователя
# ОПИСАНИЕ: Полный цикл: регистрация → логин → создание темы → пост → чат → профиль → выход → 401
# ЭНДПОИНТЫ: auth, topics, posts, chat, users
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: Весь сценарий выполняется без ошибок
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

echo "============================================"
echo "ТЕСТ 11: E2E сценарий пользователя"
echo "============================================"
echo ""

RAND_SUFFIX=$RANDOM
NEW_USER="E2EUser_$RAND_SUFFIX"
NEW_EMAIL="e2e_$RAND_SUFFIX@example.com"

# --- Шаг 1: Регистрация ---
echo "=== Шаг 1: Регистрация нового пользователя ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/register" \
  -H "Content-Type: application/json" \
  -d "{\"username\":\"$NEW_USER\",\"email\":\"$NEW_EMAIL\",\"password\":\"E2EPass123!\"}")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
  echo "✅ PASS: Регистрация успешна (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Регистрация вернула HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Шаг 2: Вход и получение токена ---
echo ""
echo "=== Шаг 2: Вход и получение токена ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d "{\"usernameOrEmail\":\"$NEW_USER\",\"password\":\"E2EPass123!\"}")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  TOKEN=$(echo "$BODY" | jq -r '.token // empty')
  USER_ID=$(echo "$BODY" | jq -r '.userId // empty')
  if [ -n "$TOKEN" ] && [ "$TOKEN" != "null" ]; then
    echo "✅ PASS: Вход успешен, токен получен (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
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

# --- Шаг 3: Создание темы ---
echo ""
echo "=== Шаг 3: Создание темы ==="
if [ -n "$TOKEN" ]; then
  # Получаем sectionId
  SECTION_ID=$(curl -s "$BASE_URL/api/sections" | jq -r '.[0].id // 1')

  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/topics" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"sectionId\":$SECTION_ID,\"title\":\"E2E тема $RAND_SUFFIX\",\"content\":\"Создана в E2E тесте\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)
  BODY=$(echo "$RESPONSE" | head -n -1)

  if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    TOPIC_ID=$(echo "$BODY" | jq -r '.id')
    echo "✅ PASS: Тема создана (HTTP $HTTP_CODE, id: $TOPIC_ID)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Создание темы вернуло HTTP $HTTP_CODE"
    echo "   Body: $BODY"
    FAILED=$((FAILED + 1))
    TOPIC_ID=""
  fi
else
  echo "❌ FAIL: Нет токена"
  FAILED=$((FAILED + 1))
  TOPIC_ID=""
fi

# --- Шаг 4: Добавление поста ---
echo ""
echo "=== Шаг 4: Добавление поста в тему ==="
if [ -n "$TOKEN" ] && [ -n "$TOPIC_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/posts" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"topicId\":$TOPIC_ID,\"content\":\"E2E пост в теме $TOPIC_ID\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)
  BODY=$(echo "$RESPONSE" | head -n -1)

  if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    POST_ID=$(echo "$BODY" | jq -r '.id')
    echo "✅ PASS: Пост создан (HTTP $HTTP_CODE, id: $POST_ID)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Создание поста вернуло HTTP $HTTP_CODE"
    echo "   Body: $BODY"
    FAILED=$((FAILED + 1))
    POST_ID=""
  fi
else
  echo "⚠️  SKIP: Нет токена или ID темы"
  POST_ID=""
fi

# --- Шаг 5: Редактирование поста ---
echo ""
echo "=== Шаг 5: Редактирование поста ==="
if [ -n "$TOKEN" ] && [ -n "$POST_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/posts/$POST_ID" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"content\":\"E2E отредактированный пост $(date +%s)\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Пост отредактирован (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Редактирование вернуло HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет токена или ID поста"
fi

# --- Шаг 6: Отправка сообщения в чат ---
echo ""
echo "=== Шаг 6: Отправка сообщения в чат ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/chat" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"content\":\"E2E чат-сообщение от $NEW_USER\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    echo "✅ PASS: Сообщение в чат отправлено (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Отправка чата вернула HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет токена"
fi

# --- Шаг 7: Просмотр своего профиля ---
echo ""
echo "=== Шаг 7: Просмотр профиля ==="
if [ -n "$TOKEN" ] && [ -n "$USER_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/users/$USER_ID")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Профиль просмотрен (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Профиль вернул HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет токена или ID пользователя"
fi

# --- Шаг 8: Попытка создать тему без токена ---
echo ""
echo "=== Шаг 8: Попытка создать тему без токена → 401 ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/topics" \
  -H "Content-Type: application/json" \
  -d '{"sectionId":1,"title":"Неавторизованная тема","content":"Должна вернуть 401"}')

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "401" ]; then
  echo "✅ PASS: Создание темы без токена — 401 (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 401, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Итог ---
echo ""
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 11 (E2E):"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
