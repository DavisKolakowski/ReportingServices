import { SqlDataType, HtmlInputType } from '@/models/enums';

export const getDataType = (sqlType: SqlDataType) => {
  switch (sqlType) {
    case SqlDataType.BigInt:
    case SqlDataType.Int:
    case SqlDataType.SmallInt:
    case SqlDataType.TinyInt:
      return Number;
    case SqlDataType.Binary:
    case SqlDataType.Image:
    case SqlDataType.Timestamp:
    case SqlDataType.VarBinary:
      return Uint8Array;
    case SqlDataType.Bit:
      return Boolean;
    case SqlDataType.Char:
    case SqlDataType.NChar:
    case SqlDataType.NText:
    case SqlDataType.NVarChar:
    case SqlDataType.Text:
    case SqlDataType.VarChar:
      return String;
    case SqlDataType.Date:
    case SqlDataType.DateTime:
    case SqlDataType.DateTime2:
    case SqlDataType.DateTimeOffset:
    case SqlDataType.SmallDateTime:
      return Date;
    case SqlDataType.Decimal:
    case SqlDataType.Money:
    case SqlDataType.Numeric:
    case SqlDataType.SmallMoney:
    case SqlDataType.Float:
    case SqlDataType.Real:
      return Number;
    case SqlDataType.Time:
      return String;
    case SqlDataType.UniqueIdentifier:
      return String;
    default:
      return String;
  }
};

export const getInputType = (sqlType: SqlDataType): HtmlInputType => {
  switch (sqlType) {
    case SqlDataType.BigInt:
    case SqlDataType.Int:
    case SqlDataType.SmallInt:
    case SqlDataType.TinyInt:
    case SqlDataType.Decimal:
    case SqlDataType.Money:
    case SqlDataType.Numeric:
    case SqlDataType.SmallMoney:
    case SqlDataType.Float:
    case SqlDataType.Real:
      return HtmlInputType.Number;
    case SqlDataType.Bit:
      return HtmlInputType.Checkbox;
    case SqlDataType.Date:
    case SqlDataType.DateTime:
    case SqlDataType.DateTime2:
    case SqlDataType.DateTimeOffset:
    case SqlDataType.SmallDateTime:
      return HtmlInputType.Date;
    case SqlDataType.Time:
      return HtmlInputType.Time;
    default:
      return HtmlInputType.Text;
  }
};
