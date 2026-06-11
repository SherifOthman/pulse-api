using Pulse.API.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Pulse.API.Services;

public static class CitySeeder
{
    private static readonly Guid CairoId       = Guid.Parse("00000001-0000-0000-0000-000000000000");
    private static readonly Guid GizaId        = Guid.Parse("00000002-0000-0000-0000-000000000000");
    private static readonly Guid AlexId        = Guid.Parse("00000003-0000-0000-0000-000000000000");
    private static readonly Guid DakahliaId    = Guid.Parse("00000004-0000-0000-0000-000000000000");
    private static readonly Guid SharqiaId     = Guid.Parse("00000005-0000-0000-0000-000000000000");
    private static readonly Guid QalyubiaId    = Guid.Parse("00000006-0000-0000-0000-000000000000");
    private static readonly Guid GharbiaId     = Guid.Parse("00000007-0000-0000-0000-000000000000");
    private static readonly Guid MenoufiaId    = Guid.Parse("00000008-0000-0000-0000-000000000000");
    private static readonly Guid BeheiraId     = Guid.Parse("00000009-0000-0000-0000-000000000000");
    private static readonly Guid KafrId        = Guid.Parse("0000000A-0000-0000-0000-000000000000");
    private static readonly Guid DamiettaId    = Guid.Parse("0000000B-0000-0000-0000-000000000000");
    private static readonly Guid MinyaId       = Guid.Parse("0000000C-0000-0000-0000-000000000000");
    private static readonly Guid AsyutId       = Guid.Parse("0000000D-0000-0000-0000-000000000000");
    private static readonly Guid SohagId       = Guid.Parse("0000000E-0000-0000-0000-000000000000");
    private static readonly Guid QenaId        = Guid.Parse("0000000F-0000-0000-0000-000000000000");
    private static readonly Guid LuxorId       = Guid.Parse("00000010-0000-0000-0000-000000000000");
    private static readonly Guid AswanId       = Guid.Parse("00000011-0000-0000-0000-000000000000");
    private static readonly Guid RedSeaId      = Guid.Parse("00000012-0000-0000-0000-000000000000");
    private static readonly Guid WadiId        = Guid.Parse("00000013-0000-0000-0000-000000000000");
    private static readonly Guid MatrouhId     = Guid.Parse("00000014-0000-0000-0000-000000000000");
    private static readonly Guid NorthSinaiId  = Guid.Parse("00000015-0000-0000-0000-000000000000");
    private static readonly Guid SouthSinaiId  = Guid.Parse("00000016-0000-0000-0000-000000000000");
    private static readonly Guid PortSaidId    = Guid.Parse("00000017-0000-0000-0000-000000000000");
    private static readonly Guid SuezId        = Guid.Parse("00000018-0000-0000-0000-000000000000");
    private static readonly Guid IsmailiaId    = Guid.Parse("00000019-0000-0000-0000-000000000000");
    private static readonly Guid FayoumId      = Guid.Parse("0000001A-0000-0000-0000-000000000000");
    private static readonly Guid BeniSuefId    = Guid.Parse("0000001B-0000-0000-0000-000000000000");

    private static readonly List<(Guid Id, Guid GovId, string Name)> Cities =
    [
        // القاهرة
        (Guid.Parse("10000001-0000-0000-0000-000000000000"), CairoId, "مدينة نصر"),
        (Guid.Parse("10000002-0000-0000-0000-000000000000"), CairoId, "مصر الجديدة"),
        (Guid.Parse("10000003-0000-0000-0000-000000000000"), CairoId, "العباسية"),
        (Guid.Parse("10000004-0000-0000-0000-000000000000"), CairoId, "المعادي"),
        (Guid.Parse("10000005-0000-0000-0000-000000000000"), CairoId, "حلوان"),
        (Guid.Parse("10000006-0000-0000-0000-000000000000"), CairoId, "شبرا"),
        (Guid.Parse("10000007-0000-0000-0000-000000000000"), CairoId, "الزمالك"),
        (Guid.Parse("10000008-0000-0000-0000-000000000000"), CairoId, "الدقي"),
        (Guid.Parse("10000009-0000-0000-0000-000000000000"), CairoId, "العجوزة"),
        (Guid.Parse("1000000A-0000-0000-0000-000000000000"), CairoId, "المهندسين"),
        (Guid.Parse("1000000B-0000-0000-0000-000000000000"), CairoId, "التحرير"),
        (Guid.Parse("1000000C-0000-0000-0000-000000000000"), CairoId, "وسط البلد"),
        (Guid.Parse("1000000D-0000-0000-0000-000000000000"), CairoId, "المرج"),
        (Guid.Parse("1000000E-0000-0000-0000-000000000000"), CairoId, "المطرية"),
        (Guid.Parse("1000000F-0000-0000-0000-000000000000"), CairoId, "عين شمس"),

        // الجيزة
        (Guid.Parse("20000001-0000-0000-0000-000000000000"), GizaId, "الجيزة"),
        (Guid.Parse("20000002-0000-0000-0000-000000000000"), GizaId, "العمرانية"),
        (Guid.Parse("20000003-0000-0000-0000-000000000000"), GizaId, "الهرم"),
        (Guid.Parse("20000004-0000-0000-0000-000000000000"), GizaId, "فيصل"),
        (Guid.Parse("20000005-0000-0000-0000-000000000000"), GizaId, "السادس من أكتوبر"),
        (Guid.Parse("20000006-0000-0000-0000-000000000000"), GizaId, "الشيخ زايد"),
        (Guid.Parse("20000007-0000-0000-0000-000000000000"), GizaId, "دموشية"),
        (Guid.Parse("20000008-0000-0000-0000-000000000000"), GizaId, "المنيب"),
        (Guid.Parse("20000009-0000-0000-0000-000000000000"), GizaId, "البدرشين"),
        (Guid.Parse("2000000A-0000-0000-0000-000000000000"), GizaId, "العياط"),

        // الإسكندرية
        (Guid.Parse("30000001-0000-0000-0000-000000000000"), AlexId, "وسط الإسكندرية"),
        (Guid.Parse("30000002-0000-0000-0000-000000000000"), AlexId, "سيدي جابر"),
        (Guid.Parse("30000003-0000-0000-0000-000000000000"), AlexId, "سان ستيفانو"),
        (Guid.Parse("30000004-0000-0000-0000-000000000000"), AlexId, "العصافرة"),
        (Guid.Parse("30000005-0000-0000-0000-000000000000"), AlexId, "المنتزه"),
        (Guid.Parse("30000006-0000-0000-0000-000000000000"), AlexId, "السيوف"),
        (Guid.Parse("30000007-0000-0000-0000-000000000000"), AlexId, "محرم بك"),

        // الدقهلية
        (Guid.Parse("40000001-0000-0000-0000-000000000000"), DakahliaId, "المنصورة"),
        (Guid.Parse("40000002-0000-0000-0000-000000000000"), DakahliaId, "ميت غمر"),
        (Guid.Parse("40000003-0000-0000-0000-000000000000"), DakahliaId, "دكرنس"),
        (Guid.Parse("40000004-0000-0000-0000-000000000000"), DakahliaId, "السنبلاوين"),
        (Guid.Parse("40000005-0000-0000-0000-000000000000"), DakahliaId, "بلقاس"),
        (Guid.Parse("40000006-0000-0000-0000-000000000000"), DakahliaId, "طلخا"),

        // الشرقية
        (Guid.Parse("50000001-0000-0000-0000-000000000000"), SharqiaId, "الزقازيق"),
        (Guid.Parse("50000002-0000-0000-0000-000000000000"), SharqiaId, "العاشر من رمضان"),
        (Guid.Parse("50000003-0000-0000-0000-000000000000"), SharqiaId, "بلبيس"),
        (Guid.Parse("50000004-0000-0000-0000-000000000000"), SharqiaId, "مشتول السوق"),
        (Guid.Parse("50000005-0000-0000-0000-000000000000"), SharqiaId, "أبو كبير"),
        (Guid.Parse("50000006-0000-0000-0000-000000000000"), SharqiaId, "فاقوس"),
        (Guid.Parse("50000007-0000-0000-0000-000000000000"), SharqiaId, "منيا القمح"),

        // القليوبية
        (Guid.Parse("60000001-0000-0000-0000-000000000000"), QalyubiaId, "شبرا الخيمة"),
        (Guid.Parse("60000002-0000-0000-0000-000000000000"), QalyubiaId, "بنها"),
        (Guid.Parse("60000003-0000-0000-0000-000000000000"), QalyubiaId, "قليوب"),
        (Guid.Parse("60000004-0000-0000-0000-000000000000"), QalyubiaId, "القناطر الخيرية"),
        (Guid.Parse("60000005-0000-0000-0000-000000000000"), QalyubiaId, "الخانكة"),
        (Guid.Parse("60000006-0000-0000-0000-000000000000"), QalyubiaId, "طوخ"),

        // الغربية
        (Guid.Parse("70000001-0000-0000-0000-000000000000"), GharbiaId, "طنطا"),
        (Guid.Parse("70000002-0000-0000-0000-000000000000"), GharbiaId, "المحلة الكبرى"),
        (Guid.Parse("70000003-0000-0000-0000-000000000000"), GharbiaId, "كفر الزيات"),
        (Guid.Parse("70000004-0000-0000-0000-000000000000"), GharbiaId, "زفتى"),
        (Guid.Parse("70000005-0000-0000-0000-000000000000"), GharbiaId, "السمنود"),

        // المنوفية
        (Guid.Parse("80000001-0000-0000-0000-000000000000"), MenoufiaId, "شبين الكوم"),
        (Guid.Parse("80000002-0000-0000-0000-000000000000"), MenoufiaId, "منوف"),
        (Guid.Parse("80000003-0000-0000-0000-000000000000"), MenoufiaId, "قويسنا"),
        (Guid.Parse("80000004-0000-0000-0000-000000000000"), MenoufiaId, "أشمون"),
        (Guid.Parse("80000005-0000-0000-0000-000000000000"), MenoufiaId, "تلا"),
        (Guid.Parse("80000006-0000-0000-0000-000000000000"), MenoufiaId, "الشهداء"),

        // البحيرة
        (Guid.Parse("90000001-0000-0000-0000-000000000000"), BeheiraId, "كفر الدوار"),
        (Guid.Parse("90000002-0000-0000-0000-000000000000"), BeheiraId, "دمنهور"),
        (Guid.Parse("90000003-0000-0000-0000-000000000000"), BeheiraId, "رشيد"),
        (Guid.Parse("90000004-0000-0000-0000-000000000000"), BeheiraId, "إدكو"),
        (Guid.Parse("90000005-0000-0000-0000-000000000000"), BeheiraId, "أبو المطامير"),
        (Guid.Parse("90000006-0000-0000-0000-000000000000"), BeheiraId, "المحمودية"),

        // كفر الشيخ
        (Guid.Parse("A0000001-0000-0000-0000-000000000000"), KafrId, "كفر الشيخ"),
        (Guid.Parse("A0000002-0000-0000-0000-000000000000"), KafrId, "بيلا"),
        (Guid.Parse("A0000003-0000-0000-0000-000000000000"), KafrId, "دسوق"),
        (Guid.Parse("A0000004-0000-0000-0000-000000000000"), KafrId, "سيدي سالم"),
        (Guid.Parse("A0000005-0000-0000-0000-000000000000"), KafrId, "مطوبس"),

        // دمياط
        (Guid.Parse("B0000001-0000-0000-0000-000000000000"), DamiettaId, "دمياط"),
        (Guid.Parse("B0000002-0000-0000-0000-000000000000"), DamiettaId, "رأس البر"),
        (Guid.Parse("B0000003-0000-0000-0000-000000000000"), DamiettaId, "فارسكور"),
        (Guid.Parse("B0000004-0000-0000-0000-000000000000"), DamiettaId, "كفر سعد"),

        // المنيا
        (Guid.Parse("C0000001-0000-0000-0000-000000000000"), MinyaId, "المنيا"),
        (Guid.Parse("C0000002-0000-0000-0000-000000000000"), MinyaId, "ملوي"),
        (Guid.Parse("C0000003-0000-0000-0000-000000000000"), MinyaId, "أبو قرقاص"),
        (Guid.Parse("C0000004-0000-0000-0000-000000000000"), MinyaId, "بني مزار"),
        (Guid.Parse("C0000005-0000-0000-0000-000000000000"), MinyaId, "مطاي"),
        (Guid.Parse("C0000006-0000-0000-0000-000000000000"), MinyaId, "سمالوط"),

        // أسيوط
        (Guid.Parse("D0000001-0000-0000-0000-000000000000"), AsyutId, "أسيوط"),
        (Guid.Parse("D0000002-0000-0000-0000-000000000000"), AsyutId, "منفلوط"),
        (Guid.Parse("D0000003-0000-0000-0000-000000000000"), AsyutId, "أبنوب"),
        (Guid.Parse("D0000004-0000-0000-0000-000000000000"), AsyutId, "البداري"),
        (Guid.Parse("D0000005-0000-0000-0000-000000000000"), AsyutId, "ديروط"),
        (Guid.Parse("D0000006-0000-0000-0000-000000000000"), AsyutId, "القوصية"),

        // سوهاج
        (Guid.Parse("E0000001-0000-0000-0000-000000000000"), SohagId, "سوهاج"),
        (Guid.Parse("E0000002-0000-0000-0000-000000000000"), SohagId, "جرجا"),
        (Guid.Parse("E0000003-0000-0000-0000-000000000000"), SohagId, "أخميم"),
        (Guid.Parse("E0000004-0000-0000-0000-000000000000"), SohagId, "طهطا"),
        (Guid.Parse("E0000005-0000-0000-0000-000000000000"), SohagId, "طما"),
        (Guid.Parse("E0000006-0000-0000-0000-000000000000"), SohagId, "المراغة"),

        // قنا
        (Guid.Parse("F0000001-0000-0000-0000-000000000000"), QenaId, "قنا"),
        (Guid.Parse("F0000002-0000-0000-0000-000000000000"), QenaId, "نجع حمادي"),
        (Guid.Parse("F0000003-0000-0000-0000-000000000000"), QenaId, "قوص"),
        (Guid.Parse("F0000004-0000-0000-0000-000000000000"), QenaId, "دشنا"),
        (Guid.Parse("F0000005-0000-0000-0000-000000000000"), QenaId, "دندرة"),
        (Guid.Parse("F0000006-0000-0000-0000-000000000000"), QenaId, "فرشوط"),

        // الأقصر
        (Guid.Parse("10010001-0000-0000-0000-000000000000"), LuxorId, "الأقصر"),
        (Guid.Parse("10010002-0000-0000-0000-000000000000"), LuxorId, "البياضية"),
        (Guid.Parse("10010003-0000-0000-0000-000000000000"), LuxorId, "القرنة"),

        // أسوان
        (Guid.Parse("11010001-0000-0000-0000-000000000000"), AswanId, "أسوان"),
        (Guid.Parse("11010002-0000-0000-0000-000000000000"), AswanId, "كورنيش النيل"),
        (Guid.Parse("11010003-0000-0000-0000-000000000000"), AswanId, "دراو"),
        (Guid.Parse("11010004-0000-0000-0000-000000000000"), AswanId, "كوم أمبو"),
        (Guid.Parse("11010005-0000-0000-0000-000000000000"), AswanId, "إدفو"),

        // البحر الأحمر
        (Guid.Parse("12010001-0000-0000-0000-000000000000"), RedSeaId, "الغردقة"),
        (Guid.Parse("12010002-0000-0000-0000-000000000000"), RedSeaId, "سفاجا"),
        (Guid.Parse("12010003-0000-0000-0000-000000000000"), RedSeaId, "القصير"),
        (Guid.Parse("12010004-0000-0000-0000-000000000000"), RedSeaId, "مرسى علم"),
        (Guid.Parse("12010005-0000-0000-0000-000000000000"), RedSeaId, "رأس غارب"),

        // الوادي الجديد
        (Guid.Parse("13010001-0000-0000-0000-000000000000"), WadiId, "الخارجة"),
        (Guid.Parse("13010002-0000-0000-0000-000000000000"), WadiId, "الداخلة"),
        (Guid.Parse("13010003-0000-0000-0000-000000000000"), WadiId, "باريس"),

        // مطروح
        (Guid.Parse("14010001-0000-0000-0000-000000000000"), MatrouhId, "مرسى مطروح"),
        (Guid.Parse("14010002-0000-0000-0000-000000000000"), MatrouhId, "الحمام"),
        (Guid.Parse("14010003-0000-0000-0000-000000000000"), MatrouhId, "العلمين"),
        (Guid.Parse("14010004-0000-0000-0000-000000000000"), MatrouhId, "الضبعة"),
        (Guid.Parse("14010005-0000-0000-0000-000000000000"), MatrouhId, "سيوة"),

        // شمال سيناء
        (Guid.Parse("15010001-0000-0000-0000-000000000000"), NorthSinaiId, "العريش"),
        (Guid.Parse("15010002-0000-0000-0000-000000000000"), NorthSinaiId, "رفح"),
        (Guid.Parse("15010003-0000-0000-0000-000000000000"), NorthSinaiId, "بئر العبد"),
        (Guid.Parse("15010004-0000-0000-0000-000000000000"), NorthSinaiId, "الشيخ زويد"),

        // جنوب سيناء
        (Guid.Parse("16010001-0000-0000-0000-000000000000"), SouthSinaiId, "شرم الشيخ"),
        (Guid.Parse("16010002-0000-0000-0000-000000000000"), SouthSinaiId, "دهب"),
        (Guid.Parse("16010003-0000-0000-0000-000000000000"), SouthSinaiId, "نويبع"),
        (Guid.Parse("16010004-0000-0000-0000-000000000000"), SouthSinaiId, "طور سيناء"),
        (Guid.Parse("16010005-0000-0000-0000-000000000000"), SouthSinaiId, "سانت كاترين"),

        // بورسعيد
        (Guid.Parse("17010001-0000-0000-0000-000000000000"), PortSaidId, "بورسعيد"),
        (Guid.Parse("17010002-0000-0000-0000-000000000000"), PortSaidId, "بورفؤاد"),

        // السويس
        (Guid.Parse("18010001-0000-0000-0000-000000000000"), SuezId, "السويس"),
        (Guid.Parse("18010002-0000-0000-0000-000000000000"), SuezId, "عتاقة"),

        // الإسماعيلية
        (Guid.Parse("19010001-0000-0000-0000-000000000000"), IsmailiaId, "الإسماعيلية"),
        (Guid.Parse("19010002-0000-0000-0000-000000000000"), IsmailiaId, "فايد"),
        (Guid.Parse("19010003-0000-0000-0000-000000000000"), IsmailiaId, "القنطرة شرق"),
        (Guid.Parse("19010004-0000-0000-0000-000000000000"), IsmailiaId, "التل الكبير"),
        (Guid.Parse("19010005-0000-0000-0000-000000000000"), IsmailiaId, "أبو صوير"),

        // الفيوم
        (Guid.Parse("1A010001-0000-0000-0000-000000000000"), FayoumId, "الفيوم"),
        (Guid.Parse("1A010002-0000-0000-0000-000000000000"), FayoumId, "إطسا"),
        (Guid.Parse("1A010003-0000-0000-0000-000000000000"), FayoumId, "طامية"),
        (Guid.Parse("1A010004-0000-0000-0000-000000000000"), FayoumId, "سنورس"),
        (Guid.Parse("1A010005-0000-0000-0000-000000000000"), FayoumId, "أبشواي"),

        // بني سويف
        (Guid.Parse("1B010001-0000-0000-0000-000000000000"), BeniSuefId, "بني سويف"),
        (Guid.Parse("1B010002-0000-0000-0000-000000000000"), BeniSuefId, "الواسطى"),
        (Guid.Parse("1B010003-0000-0000-0000-000000000000"), BeniSuefId, "ناصر"),
        (Guid.Parse("1B010004-0000-0000-0000-000000000000"), BeniSuefId, "أهناسيا"),
        (Guid.Parse("1B010005-0000-0000-0000-000000000000"), BeniSuefId, "ببا"),
    ];

    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Set<Domain.Entities.City>().AnyAsync())
            return;

        db.Set<Domain.Entities.City>().AddRange(
            Cities.Select(c => new Domain.Entities.City { Id = c.Id, GovernorateId = c.GovId, Name = c.Name }));

        await db.SaveChangesAsync();
    }
}
