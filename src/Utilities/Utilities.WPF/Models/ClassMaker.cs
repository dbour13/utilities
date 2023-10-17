using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.WPF.Models
{
    public enum ClassMakerType { Class, ArrayClass, Property }
    /// <summary>
    /// Constructs a c# class.  Call 'ToString' to output the class as string.
    /// </summary>
    public class ClassMaker
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ClassMakerType Type { get; set; }
        public List<ClassMaker> Properties { get; set; }

        public string ValuesToString(int level = 0)
        {
            StringBuilder valueString = new StringBuilder();

            if (Type == ClassMakerType.ArrayClass)
            {
                valueString.Append(new String('\t', level));

                if (Value != null)
                {
                    valueString.Append($"{Value} = ");
                }

                valueString.Append($"new {Name}[]\n");
                valueString.Append(new String('\t', level));
                valueString.Append("{\n");

                Properties.ForEach(p =>
                {
                    valueString.Append(p.ValuesToString(level + 1) + ",\n");
                });

                // Remove the last comma
                valueString.Remove(valueString.Length - 2, 2);
                valueString.Append("\n");
                valueString.Append(new String('\t', level));
                valueString.Append("}");
            }
            else if (Type == ClassMakerType.Class)
            {
                valueString.Append(new String('\t', level));

                if (Value != null)
                {
                    valueString.Append($"{Value} = ");
                }

                valueString.Append($"new {Name}()\n");
                valueString.Append(new String('\t', level));
                valueString.Append("{\n");

                Properties.ForEach(p =>
                {
                    valueString.Append(p.ValuesToString(level + 1) + ",\n");
                });

                // Remove the last comma
                valueString.Remove(valueString.Length - 2, 2);
                valueString.Append("\n");
                valueString.Append(new String('\t', level));
                valueString.Append("}");
            }
            else
            {
                valueString.Append(new String('\t', level));

                if (!Value.GetType().IsArray)
                {
                    switch (Value.GetType().Name)
                    {
                        case nameof(String):
                            valueString.Append($"{Name} = \"{Value}\"");
                            break;
                        case nameof(Int32):
                            valueString.Append($"{Name} = {Value}");
                            break;
                        default:
                            valueString.Append($"{Name} = {Value}");
                            break;
                    }
                }
                else
                {
                    bool first = true;

                    foreach (var val in Value as IEnumerable)
                    {
                        if (first)
                        {
                            valueString.Append($"{Name} = new {val.GetType().Name}[] {{");
                            first = false;
                        }

                        switch (val.GetType().Name)
                        {
                            case nameof(String):
                                valueString.Append($"\"{val}\", ");
                                break;
                            case nameof(Int32):
                                valueString.Append($"{val}, ");
                                break;
                            default:
                                valueString.Append($"{val}, ");
                                break;
                        }
                    }
                    // Remove the last comma
                    valueString.Remove(valueString.Length - 2, 2);
                    valueString.Append("}");
                }
            }

            return valueString.ToString();
        }

        public override string ToString()
        {
            StringBuilder classString = new StringBuilder();

            if (Type == ClassMakerType.Class || Type == ClassMakerType.ArrayClass)
            {
                classString.Append($"public class {Name}\n{{\n");

                Properties.ForEach(p =>
                {
                    if (p.Type == ClassMakerType.Property)
                    {
                        classString.Append(p.ToString());
                    }
                    else if (p.Type == ClassMakerType.ArrayClass)
                    {
                        classString.Insert(0, p.Properties.FirstOrDefault().ToString() + "\n\n");

                        classString.Append($"\tpublic {p.Name}[] {p.Value} {{get;set;}}\n");
                    }
                    else
                    {
                        classString.Insert(0, p.ToString() + "\n\n");

                        classString.Append($"\tpublic {p.Name} {p.Value} {{get;set;}}\n");
                    }
                });

                classString.Append("}");
            }
            else
            {
                classString.Append($"\tpublic {Value.GetType().Name} {Name} {{get;set;}}\n");
            }

            return classString.ToString();
        }
    }

    /* Example classmaker setup
     * 
                ClassMaker classMaker = new ClassMaker()
                {
                    Name = "ClassTest",
                    Type = ClassMakerType.Class,
                    Properties = new List<ClassMaker>()
                    {
                        new ClassMaker()
                        {
                            Name = "StringTest",
                            Value = "Test",
                            Type = ClassMakerType.Property
                        },
                        new ClassMaker()
                        {
                            Name = "IntTest",
                            Value = 123,
                            Type = ClassMakerType.Property
                        },
                        new ClassMaker()
                        {
                            Name = "ArrayTest",
                            Value = new int[] {1,2,3},
                            Type = ClassMakerType.Property
                        },
                        new ClassMaker()
                        {
                            Name = "ClassTest2",
                            Value = "ObjTest",
                            Properties = new List<ClassMaker>()
                            {
                                new ClassMaker()
                                {
                                    Name = "StringTest2",
                                    Value = "Test2",
                                    Type = ClassMakerType.Property
                                }
                            },
                            Type = ClassMakerType.Class
                        },
                        new ClassMaker()
                        {
                            Name = "ClassTest3",
                            Value = "ArrayObjTest",
                            Properties = new List<ClassMaker>()
                            {
                                new ClassMaker()
                                {
                                    Name = "ClassTest3",
                                    Properties = new List<ClassMaker>()
                                    {
                                        new ClassMaker()
                                        {
                                            Name = "StringTest3",
                                            Value = "Test3",
                                            Type = ClassMakerType.Property
                                        },
                                        new ClassMaker()
                                        {
                                            Name = "IntTest2",
                                            Value = 321,
                                            Type = ClassMakerType.Property
                                        }
                                    },
                                    Type = ClassMakerType.Class
                                },
                                new ClassMaker()
                                {
                                    Name = "ClassTest3",
                                    Properties = new List<ClassMaker>()
                                    {
                                        new ClassMaker()
                                        {
                                            Name = "StringTest3",
                                            Value = "Test4",
                                            Type = ClassMakerType.Property
                                        },
                                        new ClassMaker()
                                        {
                                            Name = "IntTest2",
                                            Value = 987,
                                            Type = ClassMakerType.Property
                                        }
                                    },
                                    Type = ClassMakerType.Class
                                }
                            },
                            Type = ClassMakerType.ArrayClass
                        }

                    }
                };


                string testValue = classMaker.ToString();
                testValue += "\n" + classMaker.ValuesToString();
    */
}
