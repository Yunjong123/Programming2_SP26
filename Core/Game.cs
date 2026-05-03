using PokemonCliBattleManager.Battle;
using PokemonCliBattleManager.Items;
using PokemonCliBattleManager.Models.Moves;
using PokemonCliBattleManager.Models.Pokemon;

namespace PokemonCliBattleManager.Core;

public sealed class Game
{
    private readonly ConsoleUi _ui;
    private readonly Random _rng = new();

    public Game(ConsoleUi ui)
    {
        _ui = ui;
    }

    public void Run()
    {
        _ui.Clear();
        _ui.WriteLine("Pokemon CLI Battle and Collection Manager");
        _ui.WriteLine("DP-style turn-based battle.");
        _ui.WriteLine();

        var playerName = _ui.ReadNonEmpty("Enter trainer name: ");
        var player = new Trainer(playerName, isHuman: true);

        var pool = BuildPokemonPool();

        _ui.WriteLine();
        _ui.WriteLine("Build your party: pick 3 pokemon.");
        DraftPartySelection(player, pool);

        player.Inventory.Add(new Potion(), 5);
        player.Inventory.Add(new FullHeal(), 2);

        var enemy = new Trainer("AI Trainer", isHuman: false);
        enemy.Party.AddRange(RandomParty(pool, 2));

        // enemy.Inventory.Add(new Potion(), 2);
        // enemy.Inventory.Add(new Potion(), 2);

        _ui.WriteLine();
        _ui.WriteLine("An enemy trainer approaches!");
        _ui.Pause();

        var ctx = new BattleContext(_ui, _rng);
        var battle = new BattleSystem(ctx);

        var result = battle.RunBattle(player, enemy);

        _ui.WriteLine();
        _ui.WriteLine(result switch
        {
            BattleResult.PlayerWin => "You win!",
            BattleResult.PlayerLose => "You lose!",
            _ => "Battle ended."
        });

        _ui.Pause("Press Enter to exit.");
    }

    private List<SpeciesPokemon> BuildPokemonPool()
    {
        //Note : Names are used for flaver only. Stats are simplified.
        var list = new List<SpeciesPokemon>
        {
            new(
                name: "Pikachu",
                primaryType: PokemonType.Electric,
                secondaryType: null,
                maxHp: 110,
                attack: 55,
                defense: 40,
                spAttack: 55,
                spDefense: 50,
                speed: 90,
                moves: new IMove[]
                {
                    new DamageMove("Thunder Shock", PokemonType.Electric, power: 40, accuracy: 100, category: MoveCategory.Special),
                    new StatusMove("Thunder Wave", PokemonType.Electric, accuracy: 90, statusToApply: StatusCondition.Paralysis),
                    new DamageMove("QuickQuick Attack", PokemonType.Normal, power: 40, accuracy: 100, category: MoveCategory.Physical, priority: 1),
                    new DamageMove("Iron Tail", PokemonType.Steel, power: 80, accuracy: 75, category: MoveCategory.Physical)
                }

            ),
            new(
                name: "Chimchar",
                primaryType: PokemonType.Fire,
                secondaryType: null,
                maxHp: 120,
                attack: 65,
                defense: 45,
                spAttack: 60,
                spDefense: 45,
                speed: 65,
                moves: new IMove[]
                {
                    new DamageMove("Ember Shock", PokemonType.Fire, power: 40, accuracy: 100, category: MoveCategory.Special, statusChance: 10, statusOnHit: StatusCondition.Burn),
                    new StatusMove("Will-O-Wisp", PokemonType.Fire, accuracy: 85, statusToApply: StatusCondition.Burn),
                    new DamageMove("Flame Wheel", PokemonType.Fire, power: 60, accuracy: 100, category: MoveCategory.Physical, statusChance: 10, statusOnHit: StatusCondition.Burn),
                    new DamageMove("Scratch", PokemonType.Normal, power: 40, accuracy: 100, category: MoveCategory.Physical)
                }
            ),
            new(
                name: "Piplup",
                primaryType: PokemonType.Water,
                secondaryType: null,
                maxHp: 125,
                attack: 60,
                defense: 55,
                spAttack: 60,
                spDefense: 60,
                speed: 55,
                moves: new IMove[]
                {
                    new DamageMove("Water Pulse", PokemonType.Water, power: 60, accuracy: 100, category: MoveCategory.Special),
                    new DamageMove("Bubble", PokemonType.Water, power: 40, accuracy: 100, category: MoveCategory.Physical),
                    new DamageMove("Pound", PokemonType.Normal, power: 40, accuracy: 100, category: MoveCategory.Physical),
                    new RecoverMove("Aqua Ring", PokemonType.Water, healAmount: 20)
                }
            ),
            new(
                name: "Turtwig",
                primaryType: PokemonType.Grass,
                secondaryType: null,
                maxHp: 130,
                attack: 55,
                defense: 70,
                spAttack: 45,
                spDefense: 55,
                speed: 45,
                moves: new IMove[]
                {
                    new DamageMove("Razor Leaf", PokemonType.Grass, power: 55, accuracy: 95, category: MoveCategory.Physical),
                    new DamageMove("Seed Bomb", PokemonType.Grass, power: 80, accuracy: 100, category: MoveCategory.Physical),
                    new StatusMove("Toxic", PokemonType.Poison, accuracy: 90, statusToApply: StatusCondition.BadPoison),
                    new RecoverMove("Synthesis", PokemonType.Grass, healAmount: 25)
                }
            ),
            new(
                name: "Geadude",
                primaryType: PokemonType.Rock,
                secondaryType: PokemonType.Ground,
                maxHp: 135,
                attack: 80,
                defense: 90,
                spAttack: 30,
                spDefense: 45,
                speed: 20,
                moves: new IMove[]
                {
                    new DamageMove("Rock Slide", PokemonType.Rock, power: 75, accuracy: 90, category: MoveCategory.Physical),
                    new DamageMove("Earthquake", PokemonType.Grass, power: 100, accuracy: 100, category: MoveCategory.Physical),
                    new FieldMove("Stealth Rock", PokemonType.Rock, accuracy: 100, fieldEffect: FieldEffect.StealthRock),
                    new DamageMove("Tackle", PokemonType.Normal, power: 40, accuracy: 100, category: MoveCategory.Physical),
                }
            ),
            new(
                name: "Machop",
                primaryType: PokemonType.Fighting,
                secondaryType: null,
                maxHp: 135,
                attack: 80,
                defense: 50,
                spAttack: 35,
                spDefense: 50,
                speed: 35,
                moves: new IMove[]
                {
                    new DamageMove("Karate Chop", PokemonType.Fighting, power: 90, accuracy: 100, category: MoveCategory.Physical),
                    new DamageMove("Brick Break", PokemonType.Fighting, power: 75, accuracy: 100, category: MoveCategory.Physical),
                    new DamageMove("Rock Tomb", PokemonType.Rock, power: 60, accuracy: 95, category: MoveCategory.Physical),
                    new RecoverMove("Focus", PokemonType.Fighting, healAmount: 18)
                }
            ),
            new(
                name: "Zubat",
                primaryType: PokemonType.Poison,
                secondaryType: PokemonType.Flying,
                maxHp: 125,
                attack: 50,
                defense: 40,
                spAttack: 40,
                spDefense: 45,
                speed: 70,
                moves: new IMove[]
                {
                    new DamageMove("Wing Attack", PokemonType.Flying, power: 60, accuracy: 100, category: MoveCategory.Physical),
                    new StatusMove("Hypnosis", PokemonType.Psychic, accuracy: 75, statusToApply: StatusCondition.Sleep),
                    new DamageMove("Bite", PokemonType.Dark, power: 60, accuracy: 100, category: MoveCategory.Physical),
                    new DamageMove("Poison Fang", PokemonType.Poison, power: 50, accuracy: 100, category: MoveCategory.Physical, statusChance: 30, statusOnHit: StatusCondition.Poison)
                }
            ),
            new(
                name: "Sonver",
                primaryType: PokemonType.Grass,
                secondaryType: PokemonType.Ice,
                maxHp: 135,
                attack: 62,
                defense: 55,
                spAttack: 62,
                spDefense: 55,
                speed: 40,
                moves: new IMove[]
                {
                    new DamageMove("Ice Beam", PokemonType.Ice, power: 90, accuracy: 100, category: MoveCategory.Physical),
                    new DamageMove("Razor Leaf", PokemonType.Grass, power: 55, accuracy: 95, category: MoveCategory.Physical),
                    new StatusMove("Thunder Wave", PokemonType.Electric, accuracy: 90, statusToApply: StatusCondition.Paralysis),
                    new RecoverMove("Rest", PokemonType.Psychic, healAmount: 35, curesStatus: true),
                }
            )
        };

        return list;
    }

    private void DraftPartySelection(Trainer player, List<SpeciesPokemon> pool)
    {
        var available = pool.Select(p => p.CloneFresh()).ToList();

        while (player.Party.Count < 3)
        {
            _ui.WriteLine();
            _ui.WriteLine($"Draft progress: {player.Party.Count}/3");
            PrintPool(available);

            var pick = _ui.ReadIntInRange("Pick a number:", 1, available.Count);
            var chosen = available[pick - 1];
            player.Party.Add(chosen);
            available.RemoveAt(pick - 1);

            _ui.WriteLine($"Added {chosen.Name} to your party.");
        }
    }

    private void PrintPool(List<SpeciesPokemon> pool)
    {
        for (var i = 0; i < pool.Count; i++)
        {
            var p = pool[i];
            var types = p.SecondaryType is null ? $"{p.PrimaryType}" : $"{p.PrimaryType}/{p.SecondaryType}";
            _ui.WriteLine($"{i + 1}. {p.Name} [{types}] HP {p.MaxHp} ATK {p.Attack} DEF {p.Defense} SpA {p.SpAttack} SpD {p.SpDefense} SPD {p.Speed}");
        }
    }

    private List<SpeciesPokemon> RandomParty(List<SpeciesPokemon> pool, int count)
    {
        var shuffled = pool.Select(p => p.CloneFresh()).OrderBy(_ => _rng.Next()).ToList();
        return shuffled.Take(count).ToList();
    }
}
