﻿using System;

namespace SinefaCMD
{
    static class EnumExtensions
    {
        public static TAttribute GetAttributeValue<TAttribute>(this Enum @enum) where TAttribute : Attribute
        {            
            var fi = @enum?.GetType().GetField(@enum.ToString());

            var attribute = Attribute.GetCustomAttribute(fi, typeof (TAttribute));

            if(attribute == null)
                throw new AttributeMissingException($"Enum value ${@enum.GetType()}.{@enum} is missing a required attribute of type {typeof(TAttribute)}");

            return (TAttribute) attribute;
        }
    }
}
