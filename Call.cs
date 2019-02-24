namespace ABLProfilerConverter
{
    internal class Call
    {
        public Call(int Caller, int CallerLineNumber, int Callee)
        {
            this.Caller = Caller;
            this.CallerLineNumber = CallerLineNumber;
            this.Callee = Callee;
        }

        public int Caller { get; set; }
        public int CallerLineNumber { get; set; }
        public int Callee { get; set; }
    }
}