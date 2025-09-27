namespace Framework.PM
{
    public class C2S_PB
    {
        public class C2S_UserLogin
        {
            public string Username;
            public string Password;
            public string Code;
        }

        public class C2S_UserRegister
        {
            public string Username;
            public string Password;
            public string PhoneNo;
            public string IDCode;
            public string Code;
        }

        public class C2S_PackageInfo
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string ClientVersion { get; set; }
            public string ClientResPath { get; set; }
            public string ServerVersion { get; set; }
            public string UpdateTime { get; set; }
            public string Des { get; set; }
            public int? Size { get; set; }
            public string Author { get; set; }
        }
    }
}
