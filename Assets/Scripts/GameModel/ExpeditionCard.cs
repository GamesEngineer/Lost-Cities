using System;

namespace LostCities.GameModel
{
    public enum Expedition { INVALID = -1, WHITE, YELLOW, BLUE, GREEN, RED, /**/ COUNT };

    [Serializable]
    [System.Diagnostics.DebuggerDisplay("{Label} {typeof(LostCities.GameModel.ExpeditionCard)}")]
    public struct ExpeditionCard
    {
        public const int INVALID_VALUE = -1;
        public const int PLACEHOLDER_VALUE = 0;
        public const int WAGER_VALUE = 1;
        public const int MIN_CHECKPOINT_VALUE = 2;
        public const int MAX_CHECKPOINT_VALUE = 10;

        public Expedition Expedition { get; set; }
        public int Value { get; set; } // PLACEHOLDER [0], WAGER [1], or CHECKPOINT [2..10]

        public readonly string ValueLabel =>
            Value switch
            {
                WAGER_VALUE => "$",
                >= MIN_CHECKPOINT_VALUE and <= MAX_CHECKPOINT_VALUE => Value.ToString(),
                _ => $"<size=50%>{Expedition}"
            };
        public readonly string Label
        {
            get
            {
                if (IsValid) return $"{Expedition}_{Value}";
                if (IsPlaceholder) return Expedition.ToString();
                else return "<invalid>";
            }
        }
        public readonly bool IsValid => Value >= WAGER_VALUE && Value <= MAX_CHECKPOINT_VALUE;
        public readonly bool IsPlaceholder => Value == PLACEHOLDER_VALUE;
        public readonly bool IsWager => Value == WAGER_VALUE;

        public static readonly ExpeditionCard Invalid = new() { Expedition = Expedition.INVALID, Value = INVALID_VALUE };
        public static readonly ExpeditionCard[] Placeholders = new ExpeditionCard[]
        {
            new () { Expedition = Expedition.WHITE, Value = PLACEHOLDER_VALUE },
            new () { Expedition = Expedition.YELLOW,Value = PLACEHOLDER_VALUE },
            new () { Expedition = Expedition.BLUE,  Value = PLACEHOLDER_VALUE },
            new () { Expedition = Expedition.GREEN, Value = PLACEHOLDER_VALUE },
            new () { Expedition = Expedition.RED,   Value = PLACEHOLDER_VALUE },
        };

    }
}