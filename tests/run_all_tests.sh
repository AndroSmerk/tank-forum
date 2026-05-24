#!/bin/bash
# ============================================
# ТЕСТОВЫЙ РАННЕР TankiForum
# Запускает все тесты последовательно
# Формирует итоговый отчёт
# ============================================

BASE_URL="http://localhost:5000"
TESTS_DIR="$(dirname "$0")"
RESULTS_LOG="$TESTS_DIR/results.log"
OVERALL_PASSED=0
OVERALL_FAILED=0
OVERALL_TOTAL=0

# Цвета для вывода
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo "============================================"
echo "   ЗАПУСК ТЕСТОВ TankiForum"
echo "============================================"
echo ""

# Проверка, что сервер запущен
echo "Проверка сервера на $BASE_URL..."
SERVER_CHECK=$(curl -s -o /dev/null -w "%{http_code}" "$BASE_URL/api/categories" 2>/dev/null)

if [ "$SERVER_CHECK" = "200" ] || [ "$SERVER_CHECK" = "429" ]; then
  echo -e "${GREEN}✅ Сервер доступен${NC}"
else
  echo -e "${RED}❌ Сервер не отвечает на $BASE_URL (HTTP $SERVER_CHECK)${NC}"
  echo "   Запустите проект: dotnet run --project TankiForum"
  exit 1
fi
echo ""

# Очистка предыдущих результатов
> "$RESULTS_LOG"

# Список тестов
TEST_FILES=(
  "$TESTS_DIR/01_auth.sh"
  "$TESTS_DIR/02_categories.sh"
  "$TESTS_DIR/03_topics.sh"
  "$TESTS_DIR/04_posts.sh"
  "$TESTS_DIR/05_users.sh"
  "$TESTS_DIR/06_clans.sh"
  "$TESTS_DIR/07_messages.sh"
  "$TESTS_DIR/08_chat.sh"
  "$TESTS_DIR/09_rate_limit.sh"
  "$TESTS_DIR/10_swagger.sh"
  "$TESTS_DIR/11_e2e_scenario.sh"
)

TEST_NAMES=(
  "Аутентификация"
  "Категории"
  "Темы"
  "Посты"
  "Пользователи"
  "Кланы"
  "Личные сообщения"
  "Чат-виджет"
  "Rate Limiting"
  "Swagger"
  "E2E сценарий"
)

for i in "${!TEST_FILES[@]}"; do
  TEST_FILE="${TEST_FILES[$i]}"
  TEST_NAME="${TEST_NAMES[$i]}"
  NUM=$(printf "%02d" $((i + 1)))
  
  echo "============================================"
  echo -e "${YELLOW}ЗАПУСК ТЕСТА $NUM: $TEST_NAME${NC}"
  echo "============================================"
  
  if [ -f "$TEST_FILE" ]; then
    bash "$TEST_FILE"
    EXIT_CODE=$?
    
    if [ "$EXIT_CODE" -eq 0 ]; then
      echo -e "${GREEN}✅ ТЕСТ $NUM: $TEST_NAME — ПРОЙДЕН${NC}" | tee -a "$RESULTS_LOG"
    else
      echo -e "${RED}❌ ТЕСТ $NUM: $TEST_NAME — НЕ ПРОЙДЕН (код: $EXIT_CODE)${NC}" | tee -a "$RESULTS_LOG"
    fi
    echo ""
  else
    echo -e "${RED}❌ Файл не найден: $TEST_FILE${NC}"
    echo "ТЕСТ $NUM: $TEST_NAME — ФАЙЛ НЕ НАЙДЕН" >> "$RESULTS_LOG"
    EXIT_CODE=1
  fi
  
  # Суммируем результаты
  OVERALL_TOTAL=$((OVERALL_TOTAL + 1))
  if [ "$EXIT_CODE" -eq 0 ]; then
    OVERALL_PASSED=$((OVERALL_PASSED + 1))
  else
    OVERALL_FAILED=$((OVERALL_FAILED + 1))
  fi
done

# --- Итоговый отчёт ---
echo ""
echo "============================================"
echo "   ИТОГОВЫЙ ОТЧЁТ"
echo "============================================"
echo ""
echo "Всего тестов: $OVERALL_TOTAL"
echo -e "${GREEN}Пройдено: $OVERALL_PASSED${NC}"
echo -e "${RED}Не пройдено: $OVERALL_FAILED${NC}"

if [ "$OVERALL_TOTAL" -gt 0 ]; then
  PASS_PERCENT=$((OVERALL_PASSED * 100 / OVERALL_TOTAL))
  echo "Успешность: $PASS_PERCENT%"
fi

# Сохраняем итоговый отчёт
{
  echo ""
  echo "============================================"
  echo "   ИТОГОВЫЙ ОТЧЁТ"
  echo "============================================"
  echo "Дата: $(date)"
  echo "Всего тестов: $OVERALL_TOTAL"
  echo "Пройдено: $OVERALL_PASSED"
  echo "Не пройдено: $OVERALL_FAILED"
  if [ "$OVERALL_TOTAL" -gt 0 ]; then
    PASS_PERCENT=$((OVERALL_PASSED * 100 / OVERALL_TOTAL))
    echo "Успешность: $PASS_PERCENT%"
  fi
  echo ""
  cat "$RESULTS_LOG"
} >> "$RESULTS_LOG"

echo ""
echo "Результаты сохранены: $RESULTS_LOG"

exit $OVERALL_FAILED
