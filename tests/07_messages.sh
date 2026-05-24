#!/bin/bash
# ============================================
# ТЕСТ: Личные сообщения
# ОПИСАНИЕ: Проверка отправки, получения, чтения сообщений
# ЭНДПОИНТЫ: GET/POST/PUT /api/messages
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: Все эндпоинты требуют JWT
# ПРИМЕЧАНИЕ: Весь контроллер требует [Authorize]
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

TOKEN=$(cat /tmp/user_token.txt 2>/dev/null)
ADMIN_TOKEN=$(cat /tmp/admin_token.txt 2>/dev/null)

echo "============================================"
echo "ТЕСТ 07: Личные сообщения"
echo "============================================"

# --- Тест 7.1: Входящие (с токеном) ---
echo ""
echo "=== Тест 7.1: GET /api/messages/inbox (с токеном) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/messages/inbox" \
    -H "Authorization: Bearer $TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Входящие получены (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Нет токена"
  FAILED=$((FAILED + 1))
fi

# --- Тест 7.2: Отправленные (с токеном) ---
echo ""
echo "=== Тест 7.2: GET /api/messages/sent (с токеном) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/messages/sent" \
    -H "Authorization: Bearer $TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Отправленные получены (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Нет токена"
  FAILED=$((FAILED + 1))
fi

# --- Тест 7.3: Отправка сообщения ---
echo ""
echo "=== Тест 7.3: POST /api/messages (отправка) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/messages" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"receiverId\":1,\"subject\":\"Тест $(date +%s)\",\"content\":\"Привет, Командир!\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)
  BODY=$(echo "$RESPONSE" | head -n -1)

  if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    MSG_ID=$(echo "$BODY" | jq -r '.id // empty')
    echo "✅ PASS: Сообщение отправлено (HTTP $HTTP_CODE, id: $MSG_ID)"
    PASSED=$((PASSED + 1))
    echo "$MSG_ID" > /tmp/message_id.txt
  else
    echo "❌ FAIL: Отправка сообщения вернула HTTP $HTTP_CODE"
    echo "   Body: $BODY"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Нет токена"
  FAILED=$((FAILED + 1))
fi

# --- Тест 7.4: Отправка пустого сообщения ---
echo ""
echo "=== Тест 7.4: POST /api/messages (пустое) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/messages" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"receiverId\":1,\"subject\":\"\",\"content\":\"\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "400" ]; then
    echo "✅ PASS: Пустое сообщение — 400 (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 400, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет токена"
fi

# --- Тест 7.5: Получение конкретного сообщения ---
echo ""
echo "=== Тест 7.5: GET /api/messages/{id} ==="
MSG_ID=$(cat /tmp/message_id.txt 2>/dev/null)
if [ -n "$TOKEN" ] && [ -n "$MSG_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/messages/$MSG_ID" \
    -H "Authorization: Bearer $TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Сообщение #$MSG_ID получено (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID сообщения или токена"
fi

# --- Тест 7.6: Отметить как прочитано ---
echo ""
echo "=== Тест 7.6: PUT /api/messages/{id}/read ==="
if [ -n "$ADMIN_TOKEN" ] && [ -n "$MSG_ID" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X PUT "$BASE_URL/api/messages/$MSG_ID/read" \
    -H "Authorization: Bearer $ADMIN_TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "204" ]; then
    echo "✅ PASS: Сообщение отмечено как прочитанное (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 204, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "⚠️  SKIP: Нет ID сообщения или админ-токена"
fi

# --- Итог ---
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 07:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
