using Winch.Util;

namespace DeepSubmergence
{
    public static class Enums
    {
        [EnumHolder]
        public static class ItemSubtypes
        {
            public static readonly ItemSubtype PUMP;
            public static readonly ItemSubtype PRESSURE_VESSEL;
        }

        [EnumHolder]
        public static class BuildingTiers
        {
            public static readonly BuildingTierId SEA_BASE_TIER_1;
            public static readonly BuildingTierId SEA_BASE_TIER_2;
            public static readonly BuildingTierId SEA_BASE_TIER_3;
        }

        [EnumHolder]
        public static class GridKeys
        {
            public static readonly GridKey SEA_BASE_BLUEPRINT_OUTPUT;
            public static readonly GridKey SEA_BASE_TIER_1;
            public static readonly GridKey SEA_BASE_TIER_2;
            public static readonly GridKey SEA_BASE_TIER_3;
            public static readonly GridKey SEA_BASE_PRESSURE_VESSEL_TIER_1_RECIPE;
            public static readonly GridKey SEA_BASE_PRESSURE_VESSEL_TIER_2_RECIPE;
            public static readonly GridKey SEA_BASE_PRESSURE_VESSEL_TIER_3_RECIPE;
            public static readonly GridKey SEA_BASE_PUMP_TIER_1_RECIPE;
            public static readonly GridKey SEA_BASE_PUMP_TIER_2_RECIPE;
            public static readonly GridKey SEA_BASE_PUMP_TIER_3_RECIPE;
            public static readonly GridKey SEA_BASE_PRODUCT;
            public static readonly GridKey SEA_BASE_PUMPS;
            public static readonly GridKey SEA_BASE_PRESSURE_VESSELS;
            public static readonly GridKey HOLLOW_DIVER_QUEST0;
            public static readonly GridKey HOLLOW_DIVER_QUEST1;
            public static readonly GridKey HOLLOW_DIVER_QUEST2;
            public static readonly GridKey HOLLOW_DIVER_QUEST3;
        }
    }
}
