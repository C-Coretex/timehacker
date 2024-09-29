namespace TimeHacker.Domain.Tests.Helpers
{
    public static class TheoryDataHelpers
    {
        public static TheoryData<bool, bool> TwoBoolPermutationData =>
            new()
            {
                { false, false },
                { true, false },
                { false, true },
                { true, true }
            };

        public static TheoryData<bool, bool, bool> ThreeBoolPermutationData =>
            new()
            {
                { false, false, false },
                { false, false, true },
                { false, true, false },
                { false, true, true },
                { true, false, false },
                { true, false, true },
                { true, true, false },
                { true, true, true }
            };
    }
}
