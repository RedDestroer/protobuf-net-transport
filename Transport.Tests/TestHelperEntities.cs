using System;
using System.Collections.Generic;
using Moq;
using ProtoBuf.Transport;
using ProtoBuf.Transport.Abstract;

namespace Transport.Tests
{
    public partial class TestHelper
    {
        public class IDataPackWriterHelper
        {
            public static IDataPackWriter Create()
            {
                return CreateMock(MockBehavior.Default)
                    .Object;
            }

            public static Mock<IDataPackWriter> CreateMock(MockBehavior mockBehavior)
            {
                return new Mock<IDataPackWriter>();
            }

            public static IEnumerable<IDataPackWriter> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }
        }

        public class IDataPackReaderHelper
        {
            public static IDataPackReader Create()
            {
                return CreateMock(MockBehavior.Default)
                    .Object;
            }

            public static Mock<IDataPackReader> CreateMock(MockBehavior mockBehavior)
            {
                return new Mock<IDataPackReader>();
            }

            public static IEnumerable<IDataPackReader> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }
        }

        public class DataPairHelper
        {
            public static DataPair Create()
            {
                return Create(NextString(), NextString());
            }

            public static IEnumerable<DataPair> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }

            public static DataPair Create(string name, string value)
            {
                return new DataPair(name, value);
            }
        }

        public class HeadersHelper
        {
            public static Headers Create()
            {
                return Create(DataPairHelper.CreateEnum());
            }

            public static IEnumerable<Headers> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }

            public static Headers Create(IEnumerable<DataPair> dataPairs)
            {
                var obj = new Headers();

                if (dataPairs != null)
                {
                    foreach (var dataPair in dataPairs)
                    {
                        obj.Add(dataPair);
                    }
                }

                return obj;
            }
        }

        public class PropertiesHelper
        {
            public static Properties Create()
            {
                return Create(DataPairHelper.CreateEnum());
            }

            public static IEnumerable<Properties> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }

            public static Properties Create(IEnumerable<DataPair> dataPairs)
            {
                var obj = new Properties();

                if (dataPairs != null)
                {
                    foreach (var dataPair in dataPairs)
                    {
                        obj.AddOrReplace(dataPair);
                    }
                }

                return obj;
            }
        }

        public class DataPartHelper
        {
            public static DataPart Create()
            {
                return Create(IStreamGetterHelper.Create(), HeadersHelper.Create(), PropertiesHelper.Create());
            }

            public static IEnumerable<DataPart> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }

            public static DataPart Create(IStreamGetter streamGetter, Headers headers, Properties properties)
            {
                var obj = new DataPart(streamGetter);

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        obj.Headers.Add(header);
                    }
                }

                if (properties != null)
                {
                    foreach (var property in properties.GetPropertiesList())
                    {
                        obj.Properties.AddOrReplace(property);
                    }
                }

                return obj;
            }
        }

        public class DataPackHelper
        {
            public static DataPack Create()
            {
                return Create(NextString(), NextDateTime(), NextString(), HeadersHelper.Create(), PropertiesHelper.Create(), DataPartHelper.CreateEnum());
            }

            public static IEnumerable<DataPack> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }

            public static DataPack Create(string prefix, DateTime? dateCreate, string description, Headers headers, Properties properties, IEnumerable<DataPart> dataParts)
            {
                var obj = new DataPack(prefix);
                obj.DateCreate = dateCreate;
                obj.Description = description;

                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        obj.Headers.Add(header);
                    }
                }

                if (properties != null)
                {
                    foreach (var property in properties.GetPropertiesList())
                    {
                        obj.Properties.AddOrReplace(property);
                    }
                }

                if (dataParts != null)
                {
                    foreach (var dataPart in dataParts)
                    {
                        obj.DataParts.Add(dataPart);
                    }
                }
                
                return obj;
            }
        }

        public class ISignAlgorithmHelper
        {
            public static ISignAlgorithm Create()
            {
                return CreateMock(MockBehavior.Default)
                    .Object;
            }

            public static Mock<ISignAlgorithm> CreateMock(MockBehavior mockBehavior)
            {
                return new Mock<ISignAlgorithm>();
            }

            public static IEnumerable<ISignAlgorithm> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }
        }

        public class IStreamGetterHelper
        {
            public static IStreamGetter Create()
            {
                return CreateMock(MockBehavior.Default)
                    .Object;
            }

            public static Mock<IStreamGetter> CreateMock(MockBehavior mockBehavior)
            {
                return new Mock<IStreamGetter>();
            }

            public static IEnumerable<IStreamGetter> CreateEnum()
            {
                return new[]
                    {
                        Create(),
                        Create()
                    };
            }
        }
    }
}