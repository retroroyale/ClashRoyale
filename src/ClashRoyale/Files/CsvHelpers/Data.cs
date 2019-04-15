using System;
using System.Collections.Generic;
using System.Reflection;
using ClashRoyale.Files.CsvReader;

namespace ClashRoyale.Files.CsvHelpers
{
    public class Data
    {
        private readonly int _id;
        protected DataTable DataTable;
        protected Row Row;

        public Data(Row row, DataTable dataTable)
        {
            Row = row;
            DataTable = dataTable;
            _id = GlobalId.CreateGlobalId(dataTable.GetIndex() + 1, dataTable.Count());
        }

        public static void LoadData(Data data, Type type, Row row)
        {
            foreach (var property in type.GetProperties())
                if (property.PropertyType.IsGenericType)
                {
                    var listType = typeof(List<>);
                    var generic = property.PropertyType.GetGenericArguments();
                    var concreteType = listType.MakeGenericType(generic);
                    var newList = Activator.CreateInstance(concreteType);
                    var add = concreteType.GetMethod("Add");
                    var indexerName =
                        ((DefaultMemberAttribute) newList.GetType()
                            .GetCustomAttributes(typeof(DefaultMemberAttribute), true)[0]).MemberName;
                    var indexProperty = newList.GetType().GetProperty(indexerName);

                    for (var i = row.GetRowOffset(); i < row.GetRowOffset() + row.GetArraySize(property.Name); i++)
                    {
                        var value = row.GetValue(property.Name, i - row.GetRowOffset());

                        if (value == string.Empty && i != row.GetRowOffset())
                            value = indexProperty.GetValue(
                                newList,
                                new object[]
                                {
                                    i - row.GetRowOffset() - 1
                                }).ToString();

                        if (string.IsNullOrEmpty(value))
                        {
                            var _Object = generic[0].IsValueType ? Activator.CreateInstance(generic[0]) : string.Empty;

                            if (add != null)
                                add.Invoke(
                                    newList,
                                    new[]
                                    {
                                        _Object
                                    });
                        }
                        else
                        {
                            if (add != null)
                                add.Invoke(
                                    newList,
                                    new[]
                                    {
                                        Convert.ChangeType(value, generic[0])
                                    });
                        }
                    }

                    property.SetValue(data, newList);
                }
                else
                {
                    property.SetValue(data,
                        row.GetValue(property.Name, 0) == string.Empty
                            ? null
                            : Convert.ChangeType(row.GetValue(property.Name, 0), property.PropertyType), null);
                }
        }

        public int GetDataType()
        {
            return DataTable.GetIndex() - 3;
        }

        public int GetGlobalId()
        {
            return _id;
        }

        public int GetInstanceId()
        {
            return GlobalId.GetInstanceId(_id);
        }

        public string GetName()
        {
            return Row.GetName();
        }
    }
}