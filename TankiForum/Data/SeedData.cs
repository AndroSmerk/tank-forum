using Microsoft.EntityFrameworkCore;
using TankiForum.Models;

namespace TankiForum.Data;

public static class SeedData
{
    public static async Task Initialize(AppDbContext ctx)
    {
        if (await ctx.Users.AnyAsync()) return;

        // ===== Users =====
        var users = new List<User>
        {
            new() { Username = "Командир", Email = "komandir@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Role = "Admin", Rank = "Генерал", RankClass = "rank-general", DisplayName = "Командир", Respects = 8421, Medals = "gold,gold,gold,fire", CreatedAt = DateTime.UtcNow.AddDays(-365) },
            new() { Username = "Полковник_Медведь", Email = "polkovnik@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "★★★ Полковник", RankClass = "rank-colonel", DisplayName = "Полковник_Медведь", Vocation = "Менеджер по продажам", City = "Москва", Quote = "Броня крепка и танки наши быстры!", FavoriteTank = "E 50 M", Respects = 3421, Medals = "gold,gold,silver,fire", CreatedAt = DateTime.UtcNow.AddDays(-300) },
            new() { Username = "Tankist_1979", Email = "tankist@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "★★ Лейтенант", RankClass = "rank-lieutenant", DisplayName = "Tankist_1979", Vocation = "Системный администратор", City = "СПб", Quote = "Главное — боевой дух!", FavoriteTank = "Объект 140", Respects = 1842, Medals = "silver,fire", CreatedAt = DateTime.UtcNow.AddDays(-200) },
            new() { Username = "Serg_76", Email = "serg@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "★ Сержант", RankClass = "rank-sergeant", DisplayName = "Serg_76", Vocation = "Инженер", City = "Казань", Quote = "Работай от брони!", FavoriteTank = "IS-7", Respects = 847, Medals = "silver,fire", CreatedAt = DateTime.UtcNow.AddDays(-180) },
            new() { Username = "Vova_Na_KV2", Email = "vova@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "Ефрейтор", RankClass = "rank-corporal", DisplayName = "Vova_Na_KV2", Vocation = "Водитель", City = "ННовгород", Quote = "KV-2 лучший!", FavoriteTank = "KV-2", Respects = 312, Medals = "", CreatedAt = DateTime.UtcNow.AddDays(-90) },
            new() { Username = "Serega_Na_Objekte", Email = "serega@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "★ Сержант", RankClass = "rank-sergeant", DisplayName = "Serega_Na_Objekte", Vocation = "Предприниматель", City = "Екб", Quote = "Фарм — это искусство", FavoriteTank = "Object 252U", Respects = 1204, Medals = "gold", CreatedAt = DateTime.UtcNow.AddDays(-150) },
            new() { Username = "Maus_22", Email = "maus@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "Ефрейтор", RankClass = "rank-corporal", DisplayName = "Maus_22", Vocation = "Электрик", City = "Новосибирск", Quote = "Я — стена!", FavoriteTank = "Maus", Respects = 478, Medals = "", CreatedAt = DateTime.UtcNow.AddDays(-60) },
            new() { Username = "Рядовой_Петров", Email = "petrov@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "Рядовой", RankClass = "rank-private", DisplayName = "Рядовой_Петров", Vocation = "Охранник", City = "Воронеж", Quote = "Только учусь...", FavoriteTank = "IS-7", Respects = 47, Medals = "", CreatedAt = DateTime.UtcNow.AddDays(-30) },
            new() { Username = "WoT_forever", Email = "wotforever@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "★★ Лейтенант", RankClass = "rank-lieutenant", DisplayName = "WoT_forever", Vocation = "Программист", City = "Москва", Quote = "Теория важнее практики", FavoriteTank = "E 50 M", Respects = 2156, Medals = "gold,silver", CreatedAt = DateTime.UtcNow.AddDays(-250) },
            new() { Username = "IS7_fan", Email = "is7fan@mail.ru", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password"), Rank = "★ Сержант", RankClass = "rank-sergeant", DisplayName = "IS7_fan", Vocation = "Строитель", City = "Краснодар", Quote = "IS-7 наше всё!", FavoriteTank = "IS-7", Respects = 892, Medals = "", CreatedAt = DateTime.UtcNow.AddDays(-120) }
        };

        ctx.Users.AddRange(users);
        await ctx.SaveChangesAsync();

        // ===== Categories =====
        var categories = new List<ForumCategory>
        {
            new() { Name = "Ламповая броня", Description = "Общий чат, флудилка, патчи" },
            new() { Name = "Тактический полигон", Description = "Схемы боёв, реплеи, разбор" },
            new() { Name = "Склад нытья", Description = "Поддержка, без троллинга" }
        };

        ctx.ForumCategories.AddRange(categories);
        await ctx.SaveChangesAsync();

        // ===== Sections =====
        var sections = new List<ForumSection>
        {
            new() { CategoryId = categories[0].Id, Name = "Общий чат", Description = "Ламповое общение, обсуждения" },
            new() { CategoryId = categories[0].Id, Name = "Курилка механика-водителя", Description = "Офтоп" },
            new() { CategoryId = categories[0].Id, Name = "Архив легенд", Description = "Байки, мемы" },
            new() { CategoryId = categories[1].Id, Name = "Реплеи и гайды", Description = "Тактический разбор" },
            new() { CategoryId = categories[2].Id, Name = "Поддержка", Description = "Вент-раздел" }
        };

        ctx.ForumSections.AddRange(sections);
        await ctx.SaveChangesAsync();

        var u = (string name) => users.First(us => us.Username == name);
        var sec = (string name) => sections.First(s => s.Name == name);

        var now = DateTime.UtcNow;

        // ===== Topics & Posts =====
        // Topic 1: Правила подразделения
        var t1 = new Topic { SectionId = sec("Общий чат").Id, UserId = u("Командир").Id, Title = "📌 Правила подразделения: как не получить штрафбат", Tags = "ЗАКРЕП", CreatedAt = now.AddDays(-2) };
        ctx.Topics.Add(t1);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t1.Id, UserId = u("Командир").Id, Content = "<p>Добро пожаловать в подразделение, бойцы! Напоминаем основные правила:</p><p>1. Не оскорблять личный состав.<br>2. Не флудить в тематических разделах.<br>3. Реплеи только в Тактический полигон.<br>4. В Складе нытья — никакого троллинга, только поддержка.<br>5. За нарушения — штрафбат от 30 мин до 24 ч.</p><p>Соблюдайте устав, и бой будет в радость! ⬡</p>", CreatedAt = now.AddDays(-2).AddHours(-2) },
            new Post { TopicId = t1.Id, UserId = u("Полковник_Медведь").Id, Content = "<blockquote><strong>Командир</strong> писал(а):<br>4. В Складе нытья — никакого троллинга</blockquote><p>Принято, командир! Отличный раздел, многим помогает.</p>", CreatedAt = now.AddDays(-2).AddHours(-1) },
            new Post { TopicId = t1.Id, UserId = u("Tankist_1979").Id, Content = "<p>Правила чёткие, респект. Надо бы ещё добавить правило про рекламу — а то развелось «ускорителей».</p>", CreatedAt = now.AddDays(-1).AddHours(-4) }
        );

        // Topic 2: Патч 1.26
        var t2 = new Topic { SectionId = sec("Общий чат").Id, UserId = u("Tankist_1979").Id, Title = "Патч 1.26 — кто чего накопил? Впечатления за неделю", Tags = "ХОТОВО,#ПАТЧ", CreatedAt = now.AddDays(-3) };
        ctx.Topics.Add(t2);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t2.Id, UserId = u("Tankist_1979").Id, Content = "<p>Мужики, прошла неделя с патча 1.26. Что думаете? Я за неделю накопил 850к свободки, думаю — вкачивать новую ветку или подождать?</p><p>Лично мне нравится ребаланс ПТ, стало интереснее играть.</p>", CreatedAt = now.AddDays(-3) },
            new Post { TopicId = t2.Id, UserId = u("Serg_76").Id, Content = "<p>Я за неделю 1.2М свободки насобирал на аккаунте с премом. Вкачиваю Object 268/4 — говорят, имба после патча. Кто уже взял?</p>", CreatedAt = now.AddDays(-3).AddHours(2) },
            new Post { TopicId = t2.Id, UserId = u("Vova_Na_KV2").Id, Content = "<p>А мне патч зашёл. KV-2 бафать не стали, ну и ладно, я и так на нём 62% винрейта держу 💪😄</p>", CreatedAt = now.AddDays(-2).AddHours(-6) },
            new Post { TopicId = t2.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>Ребаланс карт — вот что важно. Прохоровку подправили, теперь centre camp — не вариант. Надо работать флангами.</p><p>Кстати, выложил реплей в полигон — гляньте.</p>", CreatedAt = now.AddDays(-1).AddHours(-3) }
        );

        // Topic 3: Фарм серебра
        var t3 = new Topic { SectionId = sec("Общий чат").Id, UserId = u("Serega_Na_Objekte").Id, Title = "На чём фармить серебро в 2026? Топ-5 машин для доната", Tags = "ГАЙД,#ФАРМ", CreatedAt = now.AddDays(-1) };
        ctx.Topics.Add(t3);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t3.Id, UserId = u("Serega_Na_Objekte").Id, Content = "<p>Ребята, всем привет! За неделю протестил 10 машин на фарм. Держите топ-5:</p><p><strong>1. Object 252U (Defender)</strong> — классика, броня держит, урон 440.<br><strong>2. E 50 M</strong> — если руки прямые, фармит космос.<br><strong>3. M54 Renegade</strong> — ДПМ зверь, но броня картон.<br><strong>4. LT-432</strong> — лучший ЛТ для фарма.<br><strong>5. Progetto 46</strong> — механика дозарядки имба.</p><p>Кто что добавит?</p>", CreatedAt = now.AddDays(-1).AddHours(-5) },
            new Post { TopicId = t3.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>Progetto 46 — да, машинка огонь. Но E 50 M если умеешь играть от брони башни — фармит стабильно 80-100к за бой.</p>", CreatedAt = now.AddDays(-1).AddHours(-3) },
            new Post { TopicId = t3.Id, UserId = u("Tankist_1979").Id, Content = "<p>Ещё TS-5 забыл! ПТ-шку за 30 баксов — броня лобовая космос, ДПМ 3000. Для фарма идеал.</p>", CreatedAt = now.AddHours(-10) }
        );

        // Topic 4: Maus
        var t4 = new Topic { SectionId = sec("Общий чат").Id, UserId = u("Maus_22").Id, Title = "Maus в 2026 — всё ещё стенка или уже бесполезен?", Tags = "#ТЯЖИ", CreatedAt = now.AddDays(-2) };
        ctx.Topics.Add(t4);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t4.Id, UserId = u("Maus_22").Id, Content = "<p>Народ, два года не играл, вернулся — а Maus уже не катит? Раньше я на нём танковал как бетонная стена, а сейчас пробивают везде голдой.</p><p>Стоит ли его сейчас вывозить или только фармить на другом?</p>", CreatedAt = now.AddDays(-2).AddHours(-4) },
            new Post { TopicId = t4.Id, UserId = u("Serg_76").Id, Content = "<p>Maus норм, но ситуативно. На городских картах — да, стена. На открытых — страдает. Голда сейчас у всех, так что чистого танкования броней уже не сделать.</p><p>Совет: играй от ХП и орудия, а не от брони.</p>", CreatedAt = now.AddDays(-2).AddHours(-2) },
            new Post { TopicId = t4.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>E 100 сейчас интереснее, честно. 750 альфы, броня норм. Но Маус — это легенда, катай если кайфуешь.</p>", CreatedAt = now.AddDays(-1).AddHours(-8) }
        );

        // Topic 5: Прицелы
        var t5 = new Topic { SectionId = sec("Общий чат").Id, UserId = u("Полковник_Медведь").Id, Title = "Как настроить прицел под себя: моды, конфиги, фишки", Tags = "ГАЙД,#МОДЫ", CreatedAt = now.AddDays(-4) };
        ctx.Topics.Add(t5);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t5.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>За 10 лет в игре перепробовал кучу прицелов. Держите лучшие настройки:</p><p><strong>1. Deegie's Scope Mod</strong> — лучший для ПТ и СТ.<br><strong>2. MeltyMap's MathMod</strong> — продвинутый прицел с калькулятором.<br><strong>3. Server crosshair</strong> — включите в настройках, must have.</p><p>Главное — уберите лишнюю анимацию, она отвлекает.</p><p>Кто каким прицелом пользуется?</p>", CreatedAt = now.AddDays(-4) },
            new Post { TopicId = t5.Id, UserId = u("Serega_Na_Objekte").Id, Content = "<p>Пользуюсь стандартным с серверным маркером. Без модов как-то привык. Но многие хвалят Deegie's — надо попробовать.</p>", CreatedAt = now.AddDays(-3).AddHours(-4) },
            new Post { TopicId = t5.Id, UserId = u("Tankist_1979").Id, Content = "<p>А я вообще на ваниле играю. Всё лишнее отключил, только серверный прицел и минималку. Так больше погружение.</p>", CreatedAt = now.AddDays(-3) }
        );

        // Topic 6: Стримы
        var t6 = new Topic { SectionId = sec("Общий чат").Id, UserId = u("Serg_76").Id, Title = "Что смотрите на стримах? Кого из танковых блогеров уважаете?", Tags = "ХОТОВО,#МЕДИА", CreatedAt = now.AddDays(-5) };
        ctx.Topics.Add(t6);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t6.Id, UserId = u("Serg_76").Id, Content = "<p>Мужики, вечерами после работы люблю стримы потанчить. Кого смотрите?</p><p>Я залипаю на:\n- Amway921 — спокойный без криков\n- The_Mogol — годные разборы\n- LeBwa — когда поржать</p>", CreatedAt = now.AddDays(-5) },
            new Post { TopicId = t6.Id, UserId = u("Tankist_1979").Id, Content = "<p>Amway — уважаю, мужик реально шарит. Ещё Strelec нравится — без воды всё по делу.</p>", CreatedAt = now.AddDays(-4).AddHours(-6) },
            new Post { TopicId = t6.Id, UserId = u("Vova_Na_KV2").Id, Content = "<p>А я Jove смотрю. Да, старый, но ламповый. Под пиво вечером — самое то 🍻</p>", CreatedAt = now.AddDays(-4).AddHours(-3) },
            new Post { TopicId = t6.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>Mogol — лучший по разборам реплеев. Гайды конкретные, без воды. Рекомендую всем, кто хочет расти в скилле.</p>", CreatedAt = now.AddDays(-3).AddHours(-2) }
        );

        // Topic 7: Разбор реплея
        var t7 = new Topic { SectionId = sec("Реплеи и гайды").Id, UserId = u("Полковник_Медведь").Id, Title = "🎯 Разбор реплея: 8 убийств на Prokhorovka в E 50 M", Tags = "РЕПЛЕЙ,#ПРОХОРОВКА,#ГАЙД", CreatedAt = now.AddHours(-10) };
        ctx.Topics.Add(t7);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t7.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>Мужики, всем салют! Разобрал сегодня реплей с Прохоровки. Основная ошибка новичков — лезут в центр на тяжах.</p><p>На E 50 M нужно работать от борта, использовать динамику.</p><p>Кинул реплей во вложения — гляньте. Кто как заезжает на этой карте?</p>", CreatedAt = now.AddHours(-10) },
            new Post { TopicId = t7.Id, UserId = u("Serg_76").Id, Content = "<blockquote><strong>Полковник_Медведь</strong> писал(а):<br>На E 50 M нужно работать от борта, использовать динамику.</blockquote><p>Согласен! Я на E 50 M уже 600 боёв. Добавлю: не забывайте про бронирование башни — можно танковать очень хорошо!</p>", CreatedAt = now.AddHours(-8) },
            new Post { TopicId = t7.Id, UserId = u("Vova_Na_KV2").Id, Content = "<p style=\"font-size:1.6rem\">🦊 🐮 🐻 🐴</p><p>— держите стикеры, мужики! 🔥</p>", CreatedAt = now.AddHours(-7) }
        );

        // Topic 8: Руинберг
        var t8 = new Topic { SectionId = sec("Реплеи и гайды").Id, UserId = u("WoT_forever").Id, Title = "Карта Руинберг: тактика за три стороны — полный разбор", Tags = "ГАЙД,#РУИНБЕРГ", CreatedAt = now.AddDays(-3) };
        ctx.Topics.Add(t8);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t8.Id, UserId = u("WoT_forever").Id, Content = "<p>Руинберг — карта, где выигрывает тот, кто контролирует центр. Разберём за каждую сторону:</p><p><strong>Верхний респ:</strong> сразу занимайте холмы слева, оттуда контроль центра.<br><strong>Нижний респ:</strong> работайте через город, не лезьте в поле.<br><strong>Респ справа:</strong> на СТ и ЛТ — проход через железку.</p><p>Главное — не стоять в кустах весь бой!</p>", CreatedAt = now.AddDays(-3) },
            new Post { TopicId = t8.Id, UserId = u("Serega_Na_Objekte").Id, Content = "<p>Хороший гайд! Добавлю: на тяжах в городе работайте от углов, не выкатывайтесь под нескольких сразу.</p><p>А на ПТ — позиции за домами в центре, там отличный прострел.</p>", CreatedAt = now.AddDays(-3).AddHours(3) }
        );

        // Topic 9: ЛТ-свет
        var t9 = new Topic { SectionId = sec("Реплеи и гайды").Id, UserId = u("IS7_fan").Id, Title = "ЛТ-свет на Малиновке: маршруты, кусты, засвет", Tags = "#ЛЁГКИЕ,#МАЛИНОВКА", CreatedAt = now.AddDays(-4) };
        ctx.Topics.Add(t9);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t9.Id, UserId = u("IS7_fan").Id, Content = "<p>Ребята, подскажите маршруты на Малиновке для ЛТ. Играю на ELC Even 90, вроде кусты знаю, но засвечивают часто.</p><p>Может кто схемы скинет?</p>", CreatedAt = now.AddDays(-4) },
            new Post { TopicId = t9.Id, UserId = u("WoT_forever").Id, Content = "<p>Главное правило Малиновки — не светись в первой фазе. Заезжай в кусты 1-й линии, жди 1-2 минуты.</p><p>На ELC: куст F6 простреливает весь центр. Потом переезд в E0 — засвет базы.</p>", CreatedAt = now.AddDays(-3).AddHours(-5) },
            new Post { TopicId = t9.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>ELC Even 90 — имба для Малиновки. Маскировка 50%+ с сетью. Совет: поставь доппайку и вентиляцию, крутись вокруг кустов не выходя из засвета.</p>", CreatedAt = now.AddDays(-3) }
        );

        // Topic 10: Об.140 vs Т-62А
        var t10 = new Topic { SectionId = sec("Реплеи и гайды").Id, UserId = u("Tankist_1979").Id, Title = "Объект 140 vs Т-62А: какой СТ брать в 2026?", Tags = "ГАЙД,#СТ", CreatedAt = now.AddDays(-6) };
        ctx.Topics.Add(t10);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t10.Id, UserId = u("Tankist_1979").Id, Content = "<p>Встал выбор: качать Об.140 или Т-62А? Оба вроде похожи, но что лучше в текущей мете?</p><p>У кого какой опыт?</p>", CreatedAt = now.AddDays(-6) },
            new Post { TopicId = t10.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>Об.140 сейчас сильнее. Углы склонения лучше, броня башни держит, ДПМ выше. Т-62А больше для коллекции.</p><p>Но если нравится классика — Т-62А тоже норм, просто сложнее.</p>", CreatedAt = now.AddDays(-5).AddHours(-4) },
            new Post { TopicId = t10.Id, UserId = u("Serega_Na_Objekte").Id, Content = "<p>Об.140 топ. Но я бы советовал сразу смотреть на K-91 — он интереснее в нынешней мете.</p>", CreatedAt = now.AddDays(-5) }
        );

        // Topic 11: Слив 15 боёв
        var t11 = new Topic { SectionId = sec("Реплеи и гайды").Id, UserId = u("Рядовой_Петров").Id, Title = "Слил 15 боёв подряд на тяжах — где я свернул не туда?", Tags = "#ПОМОЩЬ", CreatedAt = now.AddDays(-1) };
        ctx.Topics.Add(t11);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t11.Id, UserId = u("Рядовой_Петров").Id, Content = "<p>Товарищи, выручайте. Уже 15 боёв слил подряд на IS-7. Раньше норм играл, а сейчас просто кусок мяса.</p><p>Поменялось что-то в патче? Или я разучился?</p>", CreatedAt = now.AddDays(-1).AddHours(-6) },
            new Post { TopicId = t11.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>IS-7 сейчас — ситуационная машина. На городских картах заходи в первые линии, работай от башни. На открытых — не лезь, жди пока команда соберётся.</p><p>Скинь реплей пары боёв — гляну.</p>", CreatedAt = now.AddDays(-1).AddHours(-3) },
            new Post { TopicId = t11.Id, UserId = u("Tankist_1979").Id, Content = "<p>Бывает, бро. У меня тоже бывают серии по 10 сливов. Отдохни денёк, перезагрузи голову. И попробуй на СТ переключиться — помогает.</p>", CreatedAt = now.AddHours(-12) },
            new Post { TopicId = t11.Id, UserId = u("Serg_76").Id, Content = "<p>Главное — не беситься. Начинаешь психовать — играешь хуже. Сделай перерыв на 30 минут, выйди покурить/попить чай.</p><p>И проверь оборудование — может сетевая лагает.</p>", CreatedAt = now.AddHours(-2) }
        );

        // Topic 12: Сил нет (vent)
        var t12 = new Topic { SectionId = sec("Поддержка").Id, UserId = u("Рядовой_Петров").Id, Title = "Сил нет… Работа, жена, ипотека. Есть тут кто в танке?", Tags = "НЫТЬЁ,#ЖИЗНЬ,#ПОДДЕРЖКА", CreatedAt = now.AddDays(-1).AddHours(-12) };
        ctx.Topics.Add(t12);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t12.Id, UserId = u("Рядовой_Петров").Id, Content = "<p>Мужики, привет. Сижу в три ночи, не спится.</p><p>Работа — полная ***, жена пилит, ипотека до 50 лет. Единственное, что держит на плаву — вечерний взвод в WoT после того, как все уснули.</p><p>Есть тут кто в похожей ситуации? Как справляетесь?</p><p style=\"margin-top:0.5rem;font-style:italic;color:var(--text-tertiary)\">p.s. Прошу без троллинга, ребят. Просто выговориться надо.</p>", CreatedAt = now.AddDays(-1).AddHours(-12) },
            new Post { TopicId = t12.Id, UserId = u("Tankist_1979").Id, Content = "<p>Держись, братан! У многих так. Я в 36 лет сменил работу — стало легче. Главное — не замыкаться в себе. Танки — это наш способ перезагрузки.</p><p>Если нужен взвод — стучи, всегда готов подъехать. 💪</p>", CreatedAt = now.AddDays(-1).AddHours(-8) },
            new Post { TopicId = t12.Id, UserId = u("Serg_76").Id, Content = "<p>Петрович, держись! Половина форума через это прошла. Танки — лучший психотерапевт после 30.</p><p>Записывайся в экипаж, будем вместе катать и душу отводить!</p>", CreatedAt = now.AddDays(-1).AddHours(-5) }
        );

        // Topic 13: Развод (vent)
        var t13 = new Topic { SectionId = sec("Поддержка").Id, UserId = u("Serg_76").Id, Title = "Развожусь после 12 лет брака. Спасибо, что есть куда прийти", Tags = "НЫТЬЁ,#СЕМЬЯ,#ПОДДЕРЖКА", CreatedAt = now.AddDays(-2).AddHours(-4) };
        ctx.Topics.Add(t13);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t13.Id, UserId = u("Serg_76").Id, Content = "<p>Ребята, даже не знаю с чего начать. После 12 лет брака решили разойтись. Вроде и не ссорились, а как чужие стали.</p><p>Спасибо этому форуму — когда совсем тошно, заходишь сюда, читаешь темы, и легче. Хоть здесь мужики понимают.</p><p>Кто проходил через развод — как отходили?</p>", CreatedAt = now.AddDays(-2).AddHours(-4) },
            new Post { TopicId = t13.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>Крепись, Сергей. Проходил 5 лет назад. Первый год — тяжело, потом отпускает. Главное — не пить, не замыкаться.</p><p>Найди хобби (кроме танков). Спорт, рыбалка, что-то новое. А танки — вечером для души.</p>", CreatedAt = now.AddDays(-2).AddHours(-2) },
            new Post { TopicId = t13.Id, UserId = u("Tankist_1979").Id, Content = "<p>Через год станет легче, проверено. Сейчас просто дай себе время. Приходи во взвод, поболтаем.</p><p>И да, спорт реально помогает — я после развода на турники пошёл, 3 года уже.</p>", CreatedAt = now.AddDays(-1).AddHours(-10) },
            new Post { TopicId = t13.Id, UserId = u("Рядовой_Петров").Id, Content = "<p>Сергей, держись брат! Мы с тобой. В танках всегда есть место для своих.</p>", CreatedAt = now.AddDays(-1).AddHours(-4) }
        );

        // Topic 14: Сокращение (vent)
        var t14 = new Topic { SectionId = sec("Поддержка").Id, UserId = u("Tankist_1979").Id, Title = "Сократили на работе. 42 года, а чувствую себя никому не нужным", Tags = "НЫТЬЁ,#РАБОТА,#ПОДДЕРЖКА", CreatedAt = now.AddDays(-3) };
        ctx.Topics.Add(t14);
        await ctx.SaveChangesAsync();
        ctx.Posts.AddRange(
            new Post { TopicId = t14.Id, UserId = u("Tankist_1979").Id, Content = "<p>Мужики, прилетело. Сокращение в IT-отделе, попал под волну. 42 года, искать новую работу — тот ещё квест.</p><p>Жена поддерживает, но сам себя чувствую никчёмным. 15 лет отработал, а тут — раз! — и не нужен.</p><p>Было у кого такое? Как выбирались?</p>", CreatedAt = now.AddDays(-3) },
            new Post { TopicId = t14.Id, UserId = u("Полковник_Медведь").Id, Content = "<p>Tankist, бывает. Меня в 40 тоже сократили. Через 2 месяца нашёл лучше. Рынок IT сейчас оживает.</p><p>Главное — не сидеть сложа руки. Обнови резюме, пройди пару курсов. И не стесняйся просить помощи — сарафанка работает.</p><p>И да, в танках вечером отключай голову.</p>", CreatedAt = now.AddDays(-3).AddHours(3) },
            new Post { TopicId = t14.Id, UserId = u("Serg_76").Id, Content = "<p>Тоже через это прошёл в 38. Сейчас своё дело открыл, мелкий ремонт техники. Не жалею.</p><p>Кризис — это время для новых возможностей. Не ссы!</p>", CreatedAt = now.AddDays(-2).AddHours(2) },
            new Post { TopicId = t14.Id, UserId = u("Рядовой_Петров").Id, Content = "<p>Бро, 42 — это не возраст, а опыт! Ты 15 лет отпахал — это достоинство, а не недостаток.</p><p>Держись, найдёшь работу. А пока — катай с нами в танки, голову разгружать.</p>", CreatedAt = now.AddDays(-2).AddHours(5) }
        );

        await ctx.SaveChangesAsync();

        // ===== Clans =====
        var clans = new List<Clan>
        {
            new() { Name = "Тяжеловесы", Description = "Для фанатов тяжёлых танков", CreatedAt = now.AddDays(-200) },
            new() { Name = "ПТ-Снайперы", Description = "Только ПТ, только хардкор", CreatedAt = now.AddDays(-150) },
            new() { Name = "Арта-изгои", Description = "САУшники, объединяйтесь!", CreatedAt = now.AddDays(-100) }
        };

        ctx.Clans.AddRange(clans);
        await ctx.SaveChangesAsync();

        // Add Полковник_Медведь to Тяжеловесы
        ctx.UserClans.Add(new UserClan { UserId = u("Полковник_Медведь").Id, ClanId = clans[0].Id, JoinedAt = now.AddDays(-100) });
        await ctx.SaveChangesAsync();
    }
}
