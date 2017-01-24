using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Caching;
using Uni.Extensions;

namespace Uni.IoC
{
    [Serializable]
    public abstract class EntityBase //: INotifyPropertyChanged
    {
        [NonSerialized]
        private string key;

        #region INotifyPropertyChanged Members

        //public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        public string __Key { get { return key; } }

        //protected void OnPropertyChanged([CallerMemberName]string caller = null)
        //protected void OnPropertyChanged(string caller = null)
        //{
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(caller));
        //}

        //protected bool SetField<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        //protected bool SetField<T>(ref T field, T value, string propertyName = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        //    field = value;
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

        public EntityBase()
        {
            key = Guid.NewGuid().ToString("N");
        }
    }

    public class ModelData
    {
        private static Uni.IoC.UniIoC container = new Uni.IoC.UniIoC();
        private static Type entityBaseType = typeof(EntityBase);
        private static Type collectionType = typeof(ICollection);

        static ModelData()
        {
        }

        public static dynamic New(string name)
        {
            IModelEntity model = Model.Get(name);

            Type pocoType = UniTypeFactory.CreatePocoType(model);

            Type implementedInterfaceType;

            if (pocoType == null)
                throw new NullReferenceException("Poco Type cannot be created");

            implementedInterfaceType = pocoType.GetInterfaces()[0];

            if (!container.IsRegistered(name) || !container.IsRegistered(pocoType))
            {
                container.Register(ServiceCriteria.For(implementedInterfaceType).ImplementedBy(pocoType).Named(name));
            }

            object retValue = !String.IsNullOrEmpty(name) ? container.Resolve(name) : container.Resolve(pocoType);

            //var pocoInstanceCreator = Uni.IoC.InstanceFactory.CreateInstanceDelegate(pocoType);

            //object retValue = pocoInstanceCreator(null);

            return retValue;
        }

        public static dynamic Get(string key)
        {
            EntityBase entity = MemoryCache.Default.Get(key) as EntityBase;

            dynamic retVal = New("Person");

            PocoDescription pocoDesc = UniTypeFactory.GetDescription("Person");

            string propertyName;
            Type propertyType;

            //for (int i = 0; i < hashEntry.Length; i++)
            //{
            //    hashEntyItem = hashEntry[i];

            //    if (!hashEntyItem.Value.HasValue)
            //        continue;

            //    propertyName = hashEntyItem.Name.ToString();

            //    if (!pocoDesc.Properties.TryGetValue(propertyName, out propertyType))
            //        continue;

            //    if (collectionType.IsAssignableFrom(propertyType))
            //        retVal[propertyName] = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(hashEntyItem.Value), propertyType);
            //    else if (entityBaseType.IsAssignableFrom(propertyType))
            //        retVal[propertyName] = Get(hashEntyItem.Value);
            //    else
            //        retVal[propertyName] = ((IConvertible)hashEntyItem.Value).ToType(propertyType, null);

            //    //retVal[propertyName] = Convert.ChangeType(hashEntyItem.Value, pocoDesc.Properties[propertyName]);
            //}

            return retVal;
        }

        public static bool Save(object obj)
        {
            EntityBase valueBase = obj as EntityBase;

            return MemoryCache.Default.Add(valueBase.__Key, obj, DateTimeOffset.Now.AddHours(2));
        }
    }

    public static class ModelExtension
    {
        public static IModelEntity Field(this IModelEntity model, string name, string dataType = "System.String", string itemType = null, bool autoInitialize = false, string alias = null, string description = null)
        {
            ModelEntity modelEntity = model as ModelEntity;

            modelEntity.AddField(name, dataType, itemType, autoInitialize, alias);

            return modelEntity;
        }

        //public static IModelEntity Field(this IModelEntity model, string name, Type dataType = null, string itemType = null, bool autoInitialize = false, string alias = null, string description = null)
        //{
        //    ModelEntity modelEntity = model as ModelEntity;

        //    modelEntity.AddField(name, dataType, itemType, autoInitialize, alias);

        //    return modelEntity;
        //}
    }

    public class Model
    {
        static List<ModelEntity> modelList = new List<ModelEntity>();

        public static IModelEntity New(string name, string dataType = "System.String", string itemType = null, bool autoInitialize = false, string alias = null)
        {
            ModelEntity retValue = ModelEntity.New as ModelEntity;

            retValue.Name = name;
            retValue.DataTypeAsString = dataType;
            retValue.ItemType = itemType;
            retValue.Alias = alias;

            modelList.Add(retValue);

            return retValue;
        }

        public static IModelEntity Get(string name)
        {
            return modelList.FirstOrDefault(f => f.Name == name);
        }

        public static void Delete(string name)
        {
            int index = modelList.FindIndex(f => f.Name == name);

            if (index > -1)
                modelList.RemoveAt(index);
        }
    }

    public class ModelEntity : IModelEntity
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public string DataTypeAsString { get; set; }
        public Type DataType { get; set; }
        public string ItemType { get; set; }
        public bool AutoInitialize { get; set; }
        public List<ModelEntity> Fields { get; set; }

        public ModelEntity()
        {
            Fields = new List<ModelEntity>();
        }

        public static IModelEntity New
        {
            get
            {
                return new ModelEntity();
            }
        }

        public IModelEntity AddField(string name, string dataType, string itemType, bool autoInitialize = false, string alias = null, string description = null)
        {
            ModelEntity newField = ModelEntity.New as ModelEntity;

            newField.Name = name;
            newField.DataTypeAsString = dataType;
            newField.ItemType = itemType;
            newField.Alias = alias;
            newField.Description = description;
            newField.AutoInitialize = autoInitialize;

            this.Fields.Add(newField);

            return this;
        }

        public IModelEntity AddField(string name, Type dataType, string itemType, bool autoInitialize = false, string alias = null, string description = null)
        {
            ModelEntity newField = ModelEntity.New as ModelEntity;

            newField.Name = name;
            newField.DataType = dataType;
            newField.ItemType = itemType;
            newField.Alias = alias;
            newField.Description = description;
            newField.AutoInitialize = autoInitialize;

            this.Fields.Add(newField);

            return this;
        }
    }

    public interface IModelEntity
    {

    }

    public class PocoDescription
    {
        public Type PocoType { get; private set; }
        public Dictionary<string, Type> Properties { get; set; }

        public PocoDescription(Type pocoType, Dictionary<string, Type> properties)
        {
            PocoType = pocoType;
            Properties = properties;
        }
    }

    public static class UniTypeFactory
    {
        public static PocoDescription GetDescription(string name)
        {
            return MemoryCache.Default.Get(name) as PocoDescription;
        }

        public static Type CreatePocoType(IModelEntity modelEntity)
        {
            ModelEntity innerModelEntity = modelEntity as ModelEntity;
            string name = innerModelEntity.Name;

            PocoDescription pocoDescription = MemoryCache.Default.Get(name) as PocoDescription;
            Type pocoType = null;
            Dictionary<string, Type> properties = new Dictionary<string, Type>();
            if (pocoDescription == null)
            {
                Type baseType = typeof(EntityBase);
                string assemblyName = "DynamicAssembly";
                //var asmList = AppDomain.CurrentDomain.GetAssemblies();
                AssemblyBuilder asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
                ModuleBuilder moduleBuilder = asmBuilder.DefineDynamicModule(assemblyName, name + ".dll");
                TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public, baseType);

                #region Serializable Attribute

                var serializableCi = typeof(SerializableAttribute).GetConstructor(new Type[] { });
                var serializableAttributeBuilder = new CustomAttributeBuilder(serializableCi, new object[] { });

                typeBuilder.SetCustomAttribute(serializableAttributeBuilder);

                #endregion Serializable Attribute

                Type interfacePocoType = CreatePocoInterfaceType(modelEntity, moduleBuilder, typeBuilder);

                if (interfacePocoType != null)
                    typeBuilder.AddInterfaceImplementation(interfacePocoType);

                Type objectType = typeof(object);
                ConstructorBuilder constructorBuilder = null;
                ILGenerator constructorILGen = null;
                FieldBuilder privateFieldBuilder;
                PropertyBuilder propertyBuilder;
                MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;
                MethodBuilder propertyGetMethodBuilder;
                ILGenerator propertyGetMethodIL;

                MethodBuilder propertySetMethodBuilder;
                ILGenerator propertySetMethodIL;
                Type propertyType;
                MethodInfo stringCompareMi = typeof(string).GetMethod("Compare", new Type[] { typeof(string), typeof(string) });

                PropertyBuilder indexerPropertyBuilder = typeBuilder.DefineProperty("Item", PropertyAttributes.None, CallingConventions.HasThis, objectType, new Type[] { typeof(string) });

                MethodBuilder indexerGetterMethodBuilder = typeBuilder.DefineMethod("get_Item", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, objectType, new Type[] { typeof(string) });

                MethodBuilder indexerSetterMethodBuilder = typeBuilder.DefineMethod("set_Item", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName, null, new Type[] { typeof(string), objectType });

                var custNameGetIL = indexerGetterMethodBuilder.GetILGenerator();
                LocalBuilder retValLocalBuilder = custNameGetIL.DeclareLocal(objectType);
                var getterEnd = custNameGetIL.DefineLabel();

                var custNameSetIL = indexerSetterMethodBuilder.GetILGenerator();
                var setterEnd = custNameSetIL.DefineLabel();

                foreach (ModelEntity field in innerModelEntity.Fields)
                {
                    if (field.DataTypeAsString == name)
                        propertyType = typeBuilder.UnderlyingSystemType;
                    else
                        propertyType = field.DataType ?? Type.GetType(field.DataTypeAsString);

                    properties.Add(field.Name, propertyType);

                    privateFieldBuilder = typeBuilder.DefineField("_" + field.Name, propertyType, FieldAttributes.Private);

                    propertyBuilder = typeBuilder.DefineProperty(field.Name, PropertyAttributes.None, propertyType, null);

                    #region PropertyGetMethod

                    propertyGetMethodBuilder = typeBuilder.DefineMethod("get_" + field.Name, getSetAttr, propertyType, Type.EmptyTypes);

                    propertyGetMethodIL = propertyGetMethodBuilder.GetILGenerator();

                    propertyGetMethodIL.Emit(OpCodes.Ldarg_0);
                    propertyGetMethodIL.Emit(OpCodes.Ldfld, privateFieldBuilder);
                    propertyGetMethodIL.Emit(OpCodes.Ret);

                    propertyBuilder.SetGetMethod(propertyGetMethodBuilder);

                    #endregion PropertyGetMethod

                    #region PropertySetMethod

                    propertySetMethodBuilder = typeBuilder.DefineMethod("set_" + field.Name, getSetAttr, null, new Type[] { propertyType });

                    propertySetMethodIL = propertySetMethodBuilder.GetILGenerator();

                    propertySetMethodIL.Emit(OpCodes.Ldarg_0);
                    propertySetMethodIL.Emit(OpCodes.Ldarg_1);
                    propertySetMethodIL.Emit(OpCodes.Stfld, privateFieldBuilder);
                    //propertySetMethodIL.Emit(OpCodes.Call, privateFieldBuilder);
                    propertySetMethodIL.Emit(OpCodes.Ret);

                    propertyBuilder.SetSetMethod(propertySetMethodBuilder);

                    #endregion PropertySetMethod

                    #region Constructor

                    if (field.AutoInitialize)
                    {
                        if (constructorBuilder == null)
                        {
                            constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
                            constructorILGen = constructorBuilder.GetILGenerator();

                            constructorILGen.Emit(OpCodes.Ldarg_0);
                            constructorILGen.Emit(OpCodes.Call, UniConstants.ObjectCi);

                            constructorILGen.Emit(OpCodes.Ldarg_0);
                            constructorILGen.Emit(OpCodes.Call, baseType.GetConstructor(Type.EmptyTypes));
                        }

                        constructorILGen.Emit(OpCodes.Ldarg_0);

                        if (field.DataTypeAsString == name)
                            constructorILGen.Emit(OpCodes.Newobj, constructorBuilder);
                        else
                            constructorILGen.Emit(OpCodes.Newobj, propertyType.GetConstructor(Type.EmptyTypes));

                        constructorILGen.Emit(OpCodes.Call, propertySetMethodBuilder);

                        constructorILGen.Emit(OpCodes.Nop);
                    }

                    #endregion Constructor

                    #region Indexer

                    #region Indexer Getter Method

                    Label equality = custNameGetIL.DefineLabel();

                    custNameGetIL.Emit(OpCodes.Ldarg_1);
                    custNameGetIL.Emit(OpCodes.Ldstr, field.Name);
                    custNameGetIL.Emit(OpCodes.Call, stringCompareMi);
                    custNameGetIL.Emit(OpCodes.Ldc_I4_0);
                    custNameGetIL.Emit(OpCodes.Ceq);
                    custNameGetIL.Emit(OpCodes.Brfalse, equality);

                    custNameGetIL.Emit(OpCodes.Ldarg_0);
                    custNameGetIL.Emit(OpCodes.Ldfld, privateFieldBuilder);
                    if (propertyType.IsValueType)
                        custNameGetIL.Emit(OpCodes.Box, propertyType);

                    custNameGetIL.Emit(OpCodes.Stloc, retValLocalBuilder);

                    custNameGetIL.Emit(OpCodes.Br, getterEnd);

                    custNameGetIL.MarkLabel(equality);

                    #endregion

                    #region Indexer Setter Method

                    Label setterEquality = custNameSetIL.DefineLabel();

                    custNameSetIL.Emit(OpCodes.Ldarg_1);
                    custNameSetIL.Emit(OpCodes.Ldstr, field.Name);
                    custNameSetIL.Emit(OpCodes.Call, stringCompareMi);
                    custNameSetIL.Emit(OpCodes.Ldc_I4_0);
                    custNameSetIL.Emit(OpCodes.Ceq);
                    custNameSetIL.Emit(OpCodes.Brfalse, setterEquality);

                    custNameSetIL.Emit(OpCodes.Ldarg_0);
                    custNameSetIL.Emit(OpCodes.Ldarg_2);
                    custNameSetIL.Emit(OpCodes.Unbox_Any, propertyType);
                    custNameSetIL.Emit(OpCodes.Stfld, privateFieldBuilder);

                    custNameSetIL.Emit(OpCodes.Br, setterEnd);

                    custNameSetIL.MarkLabel(setterEquality);

                    #endregion Indexer Setter Method

                    #endregion Indexer
                }

                #region Indexer

                #region Indexer Getter Method

                custNameGetIL.Emit(OpCodes.Ldnull);
                custNameGetIL.Emit(OpCodes.Stloc, retValLocalBuilder);
                custNameGetIL.Emit(OpCodes.Br_S, getterEnd);

                custNameGetIL.MarkLabel(getterEnd);
                custNameGetIL.Emit(OpCodes.Ldloc, retValLocalBuilder);
                custNameGetIL.Emit(OpCodes.Ret);
                indexerPropertyBuilder.SetGetMethod(indexerGetterMethodBuilder);

                #endregion Indexer Getter Method

                #region Indexer Setter Method

                custNameSetIL.MarkLabel(setterEnd);
                custNameSetIL.Emit(OpCodes.Ret);
                indexerPropertyBuilder.SetSetMethod(indexerSetterMethodBuilder);

                #endregion Indexer Setter Method

                #endregion Indexer

                #region Constructor

                if (constructorBuilder != null)
                {
                    constructorILGen.Emit(OpCodes.Ret);
                }

                #endregion Constructor

                pocoType = typeBuilder.CreateType();

                asmBuilder.Save(name + ".dll");

                pocoDescription = new PocoDescription(pocoType, properties);

                MemoryCache.Default.Add(name, pocoDescription, DateTimeOffset.Now.AddHours(2));
            }

            return pocoDescription.PocoType;
        }

        public static Type CreatePocoInterfaceType(IModelEntity modelEntity, ModuleBuilder moduleBuilder, TypeBuilder typeBuilder)
        {
            ModelEntity innerModelEntity = modelEntity as ModelEntity;
            string name = "I" + innerModelEntity.Name;

            Type interfacePocoType = MemoryCache.Default.Get(name) as Type;
            if (interfacePocoType == null)
            {
                TypeBuilder interfaceBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public |
                                TypeAttributes.Interface |
                                TypeAttributes.Abstract);

                PropertyBuilder interfacePropertyBuilder;
                Type propertyType;

                foreach (ModelEntity field in innerModelEntity.Fields)
                {
                    if (field.DataTypeAsString == innerModelEntity.Name)
                        propertyType = typeBuilder.UnderlyingSystemType;
                    else
                        propertyType = field.DataType ?? Type.GetType(field.DataTypeAsString);

                    interfacePropertyBuilder = interfaceBuilder.DefineProperty(field.Name, PropertyAttributes.HasDefault, propertyType, null);

                    interfacePropertyBuilder.SetGetMethod(interfaceBuilder.DefineMethod("get_" + field.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual, propertyType, Type.EmptyTypes));

                    interfacePropertyBuilder.SetSetMethod(interfaceBuilder.DefineMethod("set_" + field.Name, MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Abstract | MethodAttributes.Virtual, null, new Type[] { propertyType }));
                }

                interfacePocoType = interfaceBuilder.CreateType();

                MemoryCache.Default.Add(name, interfacePocoType, DateTimeOffset.Now.AddHours(2));
            }

            return interfacePocoType;
        }
    }
}