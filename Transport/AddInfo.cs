namespace ProtoBuf.Transport
{
    [ProtoContract]
    public class AddInfo
    {
        [ProtoMember(1, IsRequired = false)]
        public int? Code { get; set; }

        [ProtoMember(2, IsRequired = false)]
        public int? ParentCode { get; set; }

        [ProtoMember(3, IsRequired = false)]
        public string Type { get; set; }

        [ProtoMember(4, IsRequired = false)]
        public string Name { get; set; }

        [ProtoMember(5, IsRequired = false)]
        public string Value { get; set; }

        [ProtoMember(6, IsRequired = false)]
        public string Note { get; set; }

        [ProtoMember(7, IsRequired = false)]
        public int? OrderBy { get; set; }
    }
}