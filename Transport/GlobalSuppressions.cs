// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ProtoBuf.Transport.DataPackReader.#Read(System.IO.Stream,System.Byte[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ProtoBuf.Transport.OfflineDataPackReader.#ReadDataParts(ProtoBuf.Transport.DataPack,System.IO.BinaryReader,System.Collections.Generic.List`1<ProtoBuf.Transport.DataPackReader+DataPartInfo>,System.IO.Stream)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ProtoBuf.Transport.OfflineDataPackWriter.#Write(ProtoBuf.Transport.DataPack,System.IO.Stream,ProtoBuf.Transport.Abstract.ISignAlgorithm)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Scope = "type", Target = "ProtoBuf.Transport.OnlineDataContainer", Justification = "NonClosingStreamWrapper is fake disposable object")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ProtoBuf.Transport.OnlineDataPackReader.#ReadDataParts(ProtoBuf.Transport.DataPack,System.IO.BinaryReader,System.Collections.Generic.List`1<ProtoBuf.Transport.DataPackReader+DataPartInfo>,System.IO.Stream)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ProtoBuf.Transport.Signer.#Sign(System.UInt32,ProtoBuf.Transport.Abstract.ISignAlgorithm,System.IO.Stream,System.IO.Stream)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ProtoBuf.Transport.Signer.#IsSignMatch(System.UInt32,ProtoBuf.Transport.Abstract.ISignAlgorithm,System.IO.Stream)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member", Target = "ProtoBuf.Transport.Signer.#RemoveSign(System.UInt32,System.IO.Stream,System.IO.Stream)")]