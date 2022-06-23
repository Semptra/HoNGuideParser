using AngleSharp;
using AngleSharp.Dom;
using System.Globalization;
using System.Text;

namespace HoNGuideParser;

class Program
{
    private const string BaseUri = "https://www.heroesofnewerth.com";
    private const string GuideAuthor = "`WhatYouGot";
    private const string GuidesDirectory = @"C:\HoN Private\HoNx64\game\guides";

    public static async Task Main()
    {
        var config = Configuration.Default.WithDefaultLoader();
        var context = BrowsingContext.New(config);

        var guidesPage = await context.OpenAsync($"{BaseUri}/guides/index");
        var heroes = guidesPage.QuerySelectorAll("a.iconHolder").Select(a =>
        {
            var title = a.QuerySelector("div.title").InnerHtml;
            var uri = $"{BaseUri}{a.GetAttribute("href")}";

            return new
            {
                Hero = title,
                HeroHoNName = HeroNameMapping[title],
                Uri = uri
            };
        });

        foreach (var hero in heroes)
        {
            Console.Write($"Processing hero [{hero.Hero}]... ");

            var heroGuidesPage = await context.OpenAsync(hero.Uri);

            var guideContainer = heroGuidesPage
                .QuerySelectorAll("a.iconHolder")
                .FirstOrDefault(a => a.QuerySelector("div.subTitle").InnerHtml.StartsWith(GuideAuthor));

            if (guideContainer == null)
            {
                Console.WriteLine($"{GuideAuthor} guide not found for hero [{hero.Hero}]");
                continue;
            }

            var guideUri = $"{BaseUri}{guideContainer.GetAttribute("href")}";
            var guidePage = await context.OpenAsync(guideUri);

            var items = guidePage.QuerySelectorAll("div.backpack");

            var startingItems = GetItems(items.Take(8));
            var essentialItems = GetItems(items.Skip(8).Take(8));
            var offensiveItems = GetItems(items.Skip(16).Take(8));
            var defensiveItems = GetItems(items.Skip(24).Take(8));

            var abilities = GetAbilities(hero.Hero, guidePage.QuerySelectorAll("div.abilHolder"));

            var heroSpecialName = HeroSpecialNameMapping.ContainsKey(hero.Hero) ? HeroSpecialNameMapping[hero.Hero] : hero.Hero;

            var guide = FormatGuide(
                hero.Hero,
                heroSpecialName,
                startingItems,
                essentialItems,
                offensiveItems,
                defensiveItems,
                abilities);

            Console.WriteLine($"Successfully parsed {GuideAuthor} guide for [{hero.Hero}]");

            var guideName = $"hero_{HeroNameMapping[hero.Hero].Replace("_", string.Empty)}_guide.txt";
            var guideSavePath = Path.Combine(GuidesDirectory, guideName);

            File.WriteAllText(guideSavePath, guide);
        }
    }

    private static List<string> GetItems(IEnumerable<IElement> items)
    {
        var result = new List<string>();

        foreach (var item in items)
        {
            var name = item.QuerySelector("img")?.GetAttribute("src")?.Split("/")?.Last()?.Split(".")?.First();

            if (name is not null)
            {
                result.Add(name);
            }
        }

        return result;
    }

    public static List<string> GetAbilities(string hero, IEnumerable<IElement> abilities)
    {
        var result = new List<string>();

        foreach (var abillity in abilities)
        {
            var icon = abillity.QuerySelector("div.icon")?.GetAttribute("style")?.Split("/").Last();

            var heroNameAbility = HeroSpecialNameMapping.ContainsKey(hero)
                ? HeroSpecialNameMapping[hero]
                : hero.Replace(" ", string.Empty);

            var ability = icon.Contains("boost")
                ? "Ability_AttributeBoost"
                : $"Ability_{heroNameAbility}{icon.Split("_").First().Last()}";

            result.Add(ability);
        }

        return result;
    }

    private static string FormatGuide(
        string heroName,
        string heroSpecialName,
        List<string> startingItems,
        List<string> essentialItems,
        List<string> offensiveItems,
        List<string> defensiveItems,
        List<string> abilities)
    {
        var stringBuilder = new StringBuilder();

        var currentDateTime = DateTime.Now.ToString("MM/dd/yy HH:mm:sstt", CultureInfo.InvariantCulture);

        stringBuilder.Append(currentDateTime);
        stringBuilder.Append("``");

        stringBuilder.Append($"Hero_{heroSpecialName}");
        stringBuilder.Append("`");

        stringBuilder.Append($"{heroName} Guide");
        stringBuilder.Append("`");

        stringBuilder.Append(heroName);
        stringBuilder.Append("`");

        stringBuilder.Append("1.00");

        AppendItems(stringBuilder, startingItems);
        AppendItems(stringBuilder, essentialItems);
        AppendItems(stringBuilder, offensiveItems);
        AppendItems(stringBuilder, defensiveItems);

        stringBuilder.Append("||||`");

        stringBuilder.Append(string.Join("|", abilities));

        stringBuilder.Append("``");

        return stringBuilder.ToString();
    }

    private static void AppendItems(StringBuilder stringBuilder, List<string> items)
    {
        stringBuilder.Append("`");

        foreach (var item in items)
        {
            stringBuilder.Append(item);
            stringBuilder.Append("|");
        }

        stringBuilder.Append("|");
    }

    private static Dictionary<string, string> HeroNameMapping = new Dictionary<string, string>
    {
        { "Accursed", "accursed" },
        { "Adrenaline", "adrenaline" },
        { "Aluna", "aluna" },
        { "Amun-Ra", "ra" },
        { "Andromeda", "andromeda" },
        { "Apex", "apex" },
        { "Arachna", "arachna" },
        { "Armadon", "armadon" },
        { "Artesia", "artesia" },
        { "Artillery", "artillery" },
        { "Balphagore", "bephelgor" },
        { "Behemoth", "behemoth" },
        { "Berzerker", "berzerker" },
        { "Blacksmith", "dwarf_magi" },
        { "Blitz", "blitz" },
        { "Blood Hunter", "hunter" },
        { "Bombardier", "bomb" },
        { "Bramble", "plant" },
        { "Bubbles", "bubbles" },
        { "Bushwack", "bushwack" },
        { "Calamity", "calamity" },
        { "Chi", "chi" },
        { "Chronos", "chronos" },
        { "Circe", "circe" },
        { "Corrupted Disciple", "corrupted_disciple" },
        { "Cthulhuphant", "cthulhuphant" },
        { "Dampeer", "dampeer" },
        { "Deadlift", "deadlift" },
        { "Deadwood", "deadwood" },
        { "Defiler", "defiler" },
        { "Demented Shaman", "shaman" },
        { "Devourer", "devourer" },
        { "Doctor Repulsor", "doctor_repulsor" },
        { "Draconis", "flamedragon" },
        { "Drunken Master", "drunkenmaster" },
        { "Electrician", "electrician" },
        { "Ellonia", "ellonia" },
        { "Emerald Warden", "emerald_warden" },
        { "Empath", "empath" },
        { "Engineer", "engineer" },
        { "Fayde", "fade" },
        { "Flint Beastwood", "flint_beastwood" },
        { "Flux", "flux" },
        { "Forsaken Archer", "forsaken_archer" },
        { "Gauntlet", "gauntlet" },
        { "Gemini", "gemini" },
        { "Geomancer", "geomancer" },
        { "Glacius", "frosty" },
        { "Goldenveil", "goldenveil" },
        { "Gravekeeper", "taint" },
        { "Grinex", "grinex" },
        { "Gunblade", "gunblade" },
        { "Hammerstorm", "hammerstorm" },
        { "Hellbringer", "hellbringer" },
        { "Ichor", "ichor" },
        { "Jeraziah", "jereziah" },
        { "Kane", "kane" },
        { "Keeper of the Forest", "treant" },
        { "Kinesis", "kenisis" },
        { "King Klout", "king_klout" },
        { "Klanx", "klanx" },
        { "Kraken", "kraken" },
        { "Legionnaire", "legionnaire" },
        { "Lodestone", "lodestone" },
        { "Lord Salforis", "dreadknight" },
        { "Magebane", "javaras" },
        { "Magmus", "magmar" },
        { "Maliken", "maliken" },
        { "Martyr", "martyr" },
        { "Master Of Arms", "master_of_arms" },
        { "Midas", "midas" },
        { "Mimix", "mimix" },
        { "Moira", "moira" },
        { "Monarch", "monarch" },
        { "Monkey King", "monkey_king" },
        { "Moon Queen", "krixi" },
        { "Moraxus", "moraxus" },
        { "Myrmidon", "hydromancer" },
        { "Night Hound", "hantumon" },
        { "Nitro", "nitro" },
        { "Nomad", "nomad" },
        { "Nymphora", "fairy" },
        { "Oogie", "oogie" },
        { "Ophelia", "ophelia" },
        { "Pandamonium", "panda" },
        { "Parallax", "parallax" },
        { "Parasite", "parasite" },
        { "Pearl", "pearl" },
        { "Pebbles", "rocky" },
        { "Pestilence", "pestilence" },
        { "Pharaoh", "mumra" },
        { "Plague Rider", "diseasedrider" },
        { "Pollywog Priest", "pollywogpriest" },
        { "Predator", "predator" },
        { "Prisoner 945", "prisoner" },
        { "Prophet", "prophet" },
        { "Puppet Master", "puppetmaster" },
        { "Pyromancer", "pyromancer" },
        { "Rally", "rally" },
        { "Rampage", "rampage" },
        { "Ravenor", "ravenor" },
        { "Revenant", "revenant" },
        { "Rhapsody", "rhapsody" },
        { "Riftwalker", "riftmage" },
        { "Riptide", "riptide" },
        { "Salomon", "salomon" },
        { "Sand Wraith", "sand_wraith" },
        { "Sapphire", "sapphire" },
        { "Scout", "scout" },
        { "Shadowblade", "shadowblade" },
        { "Shellshock", "shellshock" },
        { "Silhouette", "silhouette" },
        { "Sir Benzington", "sir_benzington" },
        { "Skrap", "skrap" },
        { "Slither", "ebulus" },
        { "Solstice", "solstice" },
        { "Soul Reaper", "helldemon" },
        { "Soulstealer", "soulstealer" },
        { "Succubus", "succubis" },
        { "Swiftblade", "hiro" },
        { "Tarot", "tarot" },
        { "Tempest", "tempest" },
        { "The Chipper", "chipper" },
        { "The Dark Lady", "vanya" },
        { "The Gladiator", "gladiator" },
        { "The Madman", "scar" },
        { "Thunderbringer", "kunas" },
        { "Torturer", "xalynx" },
        { "Tremble", "tremble" },
        { "Tundra", "tundra" },
        { "Valkyrie", "valkyrie" },
        { "Vindicator", "vindicator" },
        { "Voodoo Jester", "voodoo" },
        { "War Beast", "wolfman" },
        { "Warchief", "warchief" },
        { "Wildsoul", "yogi" },
        { "Witch Slayer", "witch_slayer" },
        { "Wretched Hag", "babayaga" },
        { "Zephyr", "zephyr" }
    };

    private static Dictionary<string, string> HeroSpecialNameMapping = new Dictionary<string, string>
    {
        { "Amun-Ra", "Ra" },
        { "Balphagore", "Bephelgor" },
        { "Blacksmith", "DwarfMagi" },
        { "Blood Hunter", "Hunter" },
        { "Bramble", "Plant" },
        { "Demented Shaman", "Shaman" },
        { "Draconis", "FlameDragon" },
        { "Fayde", "Fade" },
        { "Glacius", "Frosty" },
        { "Gravekeeper", "Taint" },
        { "Jeraziah", "Jereziah" },
        { "Keeper of the Forest", "Treant" },
        { "Kinesis", "Kenisis" },
        { "Lord Salforis", "Dreadknight" },
        { "Magebane", "Javaras" },
        { "Magmus", "Magmar" },
        { "Moon Queen", "Krixi" },
        { "Myrmidon", "Hydromancer" },
        { "Night Hound", "Hantumon" },
        { "Nymphora", "Fairy" },
        { "Pandamonium", "Panda" },
        { "Pebbles", "Rocky" },
        { "Pharaoh", "Mumra" },
        { "Plague Rider", "DiseasedRider" },
        { "Prisoner 945", "Prisoner" },
        { "Riftwalker", "Riftmage" },
        { "Shadowblade", "ShadowBlade" },
        { "Slither", "Ebulus" },
        { "Soul Reaper", "HellDemon" },
        { "Succubus", "Succubis" },
        { "Swiftblade", "Hiro" },
        { "The Chipper", "Chipper" },
        { "The Dark Lady", "Vanya" },
        { "The Gladiator", "Gladiator" },
        { "The Madman", "Scar" },
        { "Thunderbringer", "Kunas" },
        { "Torturer", "Xalynx" },
        { "Voodoo Jester", "Voodoo" },
        { "War Beast", "WolfMan" },
        { "Wildsoul", "Yogi" },
        { "Wretched Hag", "BabaYaga" }
    };
}
