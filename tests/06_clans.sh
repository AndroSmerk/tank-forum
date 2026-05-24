#!/bin/bash
# ============================================
# ТЕСТ: Кланы
# ОПИСАНИЕ: Проверка списка кланов, создания, вступления/выхода
# ЭНДПОИНТЫ: GET/POST /api/clans, POST /api/clans/create, /join, /leave
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: Корректные HTTP статусы
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

TOKEN=$(cat /tmp/user_token.txt 2>/dev/null)
ADMIN_TOKEN=$(cat /tmp/admin_token.txt 2>/dev/null)

echo "============================================"
echo "ТЕСТ 06: Кланы"
echo "============================================"

# --- Тест 6.1: Список кланов ---
echo ""
echo "=== Тест 6.1: GET /api/clans ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/clans")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)
BODY=$(echo "$RESPONSE" | head -n -1)

if [ "$HTTP_CODE" = "200" ]; then
  echo "✅ PASS: Список кланов получен (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
  echo "   Body: $BODY"
  FAILED=$((FAILED + 1))
fi

# --- Тест 6.2: Детали клана -- используем первый из списка ---
echo ""
echo "=== Тест 6.2: GET /api/clans - проверка полей ==="
CLAN_ID=$(echo "$BODY" | jq -r '.[0].id // empty' 2>/dev/null)

if [ -n "$CLAN_ID" ] && [ "$CLAN_ID" != "null" ]; then
  echo "✅ PASS: Клан найден (id: $CLAN_ID)"
  PASSED=$((PASSED + 1))
  echo "$CLAN_ID" > /tmp/clan_id.txt
else
  # Если кланов нет, создадим через API
  if [ -n "$TOKEN" ]; then
    RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/clans/create" \
      -H "Content-Type: application/json" \
      -H "Authorization: Bearer $TOKEN" \
      -d "{\"name\":\"ТестовыйКлан_$(date +%s)\",\"description\":\"Создан тестом\"}")
    HTTP_CODE=$(echo "$RESPONSE" | tail -1)
    BODY=$(echo "$RESPONSE" | head -n -1)
    if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
      CLAN_ID=$(echo "$BODY" | jq -r '.id')
      echo "$CLAN_ID" > /tmp/clan_id.txt
      echo "✅ PASS: Клан создан (HTTP $HTTP_CODE, id: $CLAN_ID)"
      PASSED=$((PASSED + 1))
    else
      echo "❌ FAIL: Не удалось получить/создать клан"
      FAILED=$((FAILED + 1))
    fi
  else
    echo "⚠️  SKIP: Нет токена для создания клана"
  fi
fi

# --- Тест 6.3: Создание клана (авторизованный) ---
echo ""
echo "=== Тест 6.3: POST /api/clans/create (с токеном) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/clans/create" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"name\":\"НовыйКлан_$(date +%s)\",\"description\":\"Описание клана\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    echo "✅ PASS: Клан создан (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Создание клана вернуло HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Нет токена"
  FAILED=$((FAILED + 1))
fi

# --- Тест 6.4: Вступление в клан ---
echo ""
echo "=== Тест 6.4: POST /api/clans/join ==="
CLAN_ID=$(cat /tmp/clan_id.txt 2>/dev/null)
if [ -n "$TOKEN" ] && [ -n "$CLAN_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/clans/join" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"clanId\":$CLAN_ID}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Заявка на вступление принята (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Вступление вернуло HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID клана или токена"
fi

# --- Тест 6.5: Выход из клана ---
echo ""
echo "=== Тест 6.5: POST /api/clans/leave ==="
if [ -n "$TOKEN" ] && [ -n "$CLAN_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/clans/leave" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"clanId\":$CLAN_ID}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Выход из клана выполнен (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Выход вернул HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID клана или токена"
fi

# --- Итог ---
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 06:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
