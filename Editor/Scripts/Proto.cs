//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: Definitions/Proto/broadcast.proto
namespace Pi.Gen
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"BroadcastProto")]
  public partial class BroadcastProto : global::ProtoBuf.IExtensible
  {
    public BroadcastProto() {}
    

    private string _Message = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Message", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Message
    {
      get { return _Message; }
      set { _Message = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}
// Generated from: Definitions/Proto/echo.proto
namespace Pi.Gen
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"EchoProto")]
  public partial class EchoProto : global::ProtoBuf.IExtensible
  {
    public EchoProto() {}
    

    private string _Message = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"Message", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string Message
    {
      get { return _Message; }
      set { _Message = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
}