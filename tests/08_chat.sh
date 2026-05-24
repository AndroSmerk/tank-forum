#!/bin/bash
# ============================================
# ТЕСТ: Чат-виджет
# ОПИСАНИЕ: Проверка отправки и получения сообщений чата
# ЭНДПОИНТЫ: GET/POST /api/chat
# ОЖИДАЕМЫЙ РЕЗУЛЬТАТ: Все эндпоинты требуют JWT
# ============================================

BASE_URL="http://localhost:5000"
PASSED=0
FAILED=0

TOKEN=$(cat /tmp/user_token.txt 2>/dev/null)

echo "============================================"
echo "ТЕСТ 08: Чат-виджет"
echo "============================================"

# --- Тест 8.1: Получение сообщений чата (с токеном) ---
echo ""
echo "=== Тест 8.1: GET /api/chat?limit=50 (с токеном) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/chat?limit=50" \
    -H "Authorization: Bearer $TOKEN")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ]; then
    echo "✅ PASS: Сообщения чата получены (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Ожидался 200, получен HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Нет токена"
  FAILED=$((FAILED + 1))
fi

# --- Тест 8.2: Отправка сообщения в чат (с токеном) ---
echo ""
echo "=== Тест 8.2: POST /api/chat (с токеном) ==="
if [ -n "$TOKEN" ]; then
  RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/chat" \
    -H "Content-Type: application/json" \
    -H "Authorization: Bearer $TOKEN" \
    -d "{\"content\":\"Тестовое сообщение чата $(date +%s)\"}")

  HTTP_CODE=$(echo "$RESPONSE" | tail -1)

  if [ "$HTTP_CODE" = "200" ] || [ "$HTTP_CODE" = "201" ]; then
    echo "✅ PASS: Сообщение отправлено в чат (HTTP $HTTP_CODE)"
    PASSED=$((PASSED + 1))
  else
    echo "❌ FAIL: Отправка чата вернула HTTP $HTTP_CODE"
    FAILED=$((FAILED + 1))
  fi
else
  echo "❌ FAIL: Нет токена"
  FAILED=$((FAILED + 1))
fi

# --- Тест 8.3: Отправка без токена ---
echo ""
echo "=== Тест 8.3: POST /api/chat (без токена) ==="
RESPONSE=$(curl -s -w "\n%{http_code}" -X POST "$BASE_URL/api/chat" \
  -H "Content-Type: application/json" \
  -d '{"content":"Без токена"}')

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "401" ]; then
  echo "✅ PASS: Без токена — 401 (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 401, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Тест 8.4: GET /api/chat без токена ---
echo ""
echo "=== Тест 8.4: GET /api/chat (без токена) ==="
RESPONSE=$(curl -s -w "\n%{http_code}" "$BASE_URL/api/chat")

HTTP_CODE=$(echo "$RESPONSE" | tail -1)

if [ "$HTTP_CODE" = "401" ]; then
  echo "✅ PASS: GET без токена — 401 (HTTP $HTTP_CODE)"
  PASSED=$((PASSED + 1))
else
  echo "❌ FAIL: Ожидался 401, получен HTTP $HTTP_CODE"
  FAILED=$((FAILED + 1))
fi

# --- Итог ---
echo ""
echo "============================================"
echo "ИТОГО ТЕСТ 08:"
echo "Пройдено: $PASSED"
echo "Не пройдено: $FAILED"
echo "============================================"
exit $FAILED
