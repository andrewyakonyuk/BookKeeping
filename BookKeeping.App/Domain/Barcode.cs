﻿
namespace BookKeeping.App.Domain
{
    public struct Barcode
    {
        public readonly string Code;
        public readonly BarcodeType Type;
        static Barcode _undefined = new Barcode(string.Empty, BarcodeType.Undefined);

        public Barcode(string code, BarcodeType type)
        {
            if (string.IsNullOrWhiteSpace(code))
                code = string.Empty;
            this.Code = code;
            this.Type = type;
        }

        public static Barcode Undefined { get { return _undefined; } }

        public override bool Equals(object obj)
        {
            if (obj is Barcode)
            {
                var other = (Barcode)obj;
                return Code == other.Code && Type == other.Type;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return 157 * Code.GetHashCode() + Type.GetHashCode();
            }
        }

        public override string ToString()
        {
            return string.Format("{0}", Code);
        }
    }

   public enum BarcodeType
   {
       Undefined
   }
}