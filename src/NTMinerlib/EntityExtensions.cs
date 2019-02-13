﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NTMiner {
    public static class EntityExtensions {
        private static readonly Dictionary<Type, PropertyInfo[]> _entityPropertiesDic = new Dictionary<Type, PropertyInfo[]>();
        public static T Update<T, TInput>(this T entity, TInput input) where T : class, IEntity<Guid> {
            if (entity == null) {
                return entity;
            }
            if (input == null) {
                return entity;
            }
            if (ReferenceEquals(entity, input)) {
                return entity;
            }
            Type entityType = entity.GetType();
            Type inputType = input.GetType();
            // 写被写对象的可写属性，且可写属性的set访问器必须数public的
            if (!_entityPropertiesDic.ContainsKey(entityType)) {
                _entityPropertiesDic.Add(entityType, entityType.GetProperties().Where(a => a.CanWrite && a.GetSetMethod(nonPublic: false) != null).ToArray());
            }
            foreach (PropertyInfo entityProperty in _entityPropertiesDic[entityType]) {
                PropertyInfo inputProperty = inputType.GetProperty(entityProperty.Name);
                if (inputProperty == null) {
                    continue;
                }
                entityProperty.SetValue(entity, inputProperty.GetValue(input, null), null);
            }

            return entity;
        }
    }
}
