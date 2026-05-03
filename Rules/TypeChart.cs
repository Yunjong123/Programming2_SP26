using System.Data;
using PokemonCliBattleManager.Models.Pokemon;

namespace PokemonCliBattleManager.Rules;

public static class TypeChart
{
    private static readonly Dictionary<(PokemonType atk, PokemonType def), double> Map = new()
    {
        //Fire
        [(PokemonType.Fire, PokemonType.Grass)] = 2.0,
        [(PokemonType.Fire, PokemonType.Ice)] = 2.0,
        [(PokemonType.Fire, PokemonType.Bug)] = 2.0,
        [(PokemonType.Fire, PokemonType.Steel)] = 2.0,
        [(PokemonType.Fire, PokemonType.Water)] = 0.5,
        [(PokemonType.Fire, PokemonType.Fire)] = 0.5,
        [(PokemonType.Fire, PokemonType.Rock)] = 0.5,

        //Water
        [(PokemonType.Water, PokemonType.Fire)] = 2.0,
        [(PokemonType.Water, PokemonType.Rock)] = 2.0,
        [(PokemonType.Water, PokemonType.Ground)] = 2.0,
        [(PokemonType.Water, PokemonType.Water)] = 0.5,
        [(PokemonType.Water, PokemonType.Grass)] = 0.5,

        //Grass
        [(PokemonType.Grass, PokemonType.Water)] = 2.0,
        [(PokemonType.Grass, PokemonType.Ground)] = 2.0,
        [(PokemonType.Grass, PokemonType.Rock)] = 2.0,
        [(PokemonType.Grass, PokemonType.Fire)] = 0.5,
        [(PokemonType.Grass, PokemonType.Grass)] = 0.5,
        [(PokemonType.Grass, PokemonType.Flying)] = 0.5,
        [(PokemonType.Grass, PokemonType.Poison)] = 0.5,
        [(PokemonType.Grass, PokemonType.Bug)] = 0.5,
        [(PokemonType.Grass, PokemonType.Steel)] = 0.5,
        [(PokemonType.Grass, PokemonType.Ice)] = 0.5,

        //Electric
        [(PokemonType.Electric, PokemonType.Water)] = 2.0,
        [(PokemonType.Electric, PokemonType.Flying)] = 2.0,
        [(PokemonType.Electric, PokemonType.Grass)] = 0.5,
        [(PokemonType.Electric, PokemonType.Electric)] = 0.5,
        [(PokemonType.Electric, PokemonType.Ground)] = 0.0,

        //Flying
        [(PokemonType.Flying, PokemonType.Grass)] = 2.0,
        [(PokemonType.Flying, PokemonType.Fighting)] = 2.0,
        [(PokemonType.Flying, PokemonType.Bug)] = 2.0,
        [(PokemonType.Flying, PokemonType.Electric)] = 0.5,
        [(PokemonType.Flying, PokemonType.Rock)] = 0.5,
        [(PokemonType.Flying, PokemonType.Steel)] = 0.5,

        //Ground
        [(PokemonType.Ground, PokemonType.Fire)] = 2.0,
        [(PokemonType.Ground, PokemonType.Electric)] = 2.0,
        [(PokemonType.Ground, PokemonType.Rock)] = 2.0,
        [(PokemonType.Ground, PokemonType.Steel)] = 2.0,
        [(PokemonType.Ground, PokemonType.Poison)] = 2.0,
        [(PokemonType.Ground, PokemonType.Grass)] = 0.5,
        [(PokemonType.Ground, PokemonType.Bug)] = 0.5,
        [(PokemonType.Ground, PokemonType.Flying)] = 0.0,

        //Rock
        [(PokemonType.Rock, PokemonType.Flying)] = 2.0,
        [(PokemonType.Rock, PokemonType.Fire)] = 2.0,
        [(PokemonType.Rock, PokemonType.Ice)] = 2.0,
        [(PokemonType.Rock, PokemonType.Bug)] = 2.0,
        [(PokemonType.Rock, PokemonType.Fighting)] = 0.5,
        [(PokemonType.Rock, PokemonType.Ground)] = 0.5,
        [(PokemonType.Rock, PokemonType.Steel)] = 0.5,

        //Fighting
        [(PokemonType.Fighting, PokemonType.Normal)] = 2.0,
        [(PokemonType.Fighting, PokemonType.Rock)] = 2.0,
        [(PokemonType.Fighting, PokemonType.Steel)] = 2.0,
        [(PokemonType.Fighting, PokemonType.Ice)] = 2.0,
        [(PokemonType.Fighting, PokemonType.Dark)] = 2.0,
        [(PokemonType.Fighting, PokemonType.Flying)] = 0.5,
        [(PokemonType.Fighting, PokemonType.Psychic)] = 0.5,
        [(PokemonType.Fighting, PokemonType.Poison)] = 0.5,
        [(PokemonType.Fighting, PokemonType.Bug)] = 0.5,

        //Ice
        [(PokemonType.Ice, PokemonType.Grass)] = 2.0,
        [(PokemonType.Ice, PokemonType.Ground)] = 2.0,
        [(PokemonType.Ice, PokemonType.Flying)] = 2.0,
        [(PokemonType.Ice, PokemonType.Dragon)] = 2.0,
        [(PokemonType.Ice, PokemonType.Fire)] = 0.5,
        [(PokemonType.Ice, PokemonType.Water)] = 0.5,
        [(PokemonType.Ice, PokemonType.Ice)] = 0.5,
        [(PokemonType.Ice, PokemonType.Steel)] = 0.5,

        //poison
        [(PokemonType.Poison, PokemonType.Grass)] = 2.0,
        [(PokemonType.Poison, PokemonType.Poison)] = 0.5,
        [(PokemonType.Poison, PokemonType.Ground)] = 0.5,
        [(PokemonType.Poison, PokemonType.Rock)] = 0.5,
        [(PokemonType.Poison, PokemonType.Steel)] = 0.0,

        //Psychic
        [(PokemonType.Psychic, PokemonType.Fighting)] = 2.0,
        [(PokemonType.Psychic, PokemonType.Poison)] = 2.0,
        [(PokemonType.Psychic, PokemonType.Steel)] = 0.5,
        [(PokemonType.Psychic, PokemonType.Psychic)] = 0.5,
        [(PokemonType.Psychic, PokemonType.Dark)] = 0.0,

        //Dark
        [(PokemonType.Dark, PokemonType.Psychic)] = 2.0,
        [(PokemonType.Dark, PokemonType.Dark)] = 0.5,
        [(PokemonType.Dark, PokemonType.Fighting)] = 0.5,

        //Steel
        [(PokemonType.Steel, PokemonType.Rock)] = 2.0,
        [(PokemonType.Steel, PokemonType.Ice)] = 2.0,
        [(PokemonType.Steel, PokemonType.Fire)] = 0.5,
        [(PokemonType.Steel, PokemonType.Water)] = 0.5,
        [(PokemonType.Steel, PokemonType.Electric)] = 0.5,
        [(PokemonType.Steel, PokemonType.Steel)] = 0.5,

        //Normal
        [(PokemonType.Normal, PokemonType.Rock)] = 0.5,
        [(PokemonType.Normal, PokemonType.Steel)] = 0.5,
    };

    public static double Effectiveness(PokemonType attackType, PokemonType defendType)
    {
        return Map.TryGetValue((attackType, defendType), out var v) ? v : 1.0;
    }

    public static double Effectiveness(PokemonType attackType, PokemonType defendType1, PokemonType? defendType2)
    {
        var m1 = Effectiveness(attackType, defendType1);
        if (defendType2 is null) return m1;

        var m2 = Effectiveness(attackType, defendType2.Value);
        return m1 * m2;
    }
}