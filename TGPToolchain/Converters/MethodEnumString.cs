using System;
using System.Globalization;
using Avalonia.Data.Converters;
using TGPToolchain.Models;

namespace TGPToolchain.Converters
{
    public class MethodEnumString : GenericValueConverter<LDBRecipe.Methods, string>
    {
        public override string Convert(LDBRecipe.Methods value, object parameter)
        {
            return value == LDBRecipe.Methods.AND ? "&&" : "||";
        }

        public override LDBRecipe.Methods ConvertBack(string value, object parameter)
        {
            return value == "&&" ? LDBRecipe.Methods.AND : LDBRecipe.Methods.OR;
        }
    }
}