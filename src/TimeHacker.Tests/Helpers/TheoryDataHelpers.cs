namespace TimeHacker.Tests.Helpers
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
    }
}
