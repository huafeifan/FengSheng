
namespace XLua
{
    public partial class LuaTable : LuaBase
    {
        public TValue GetExtension<TValue>(string key)
        {
            LuaTable table = this;
            string[] tables = key.Split('.');
            if (tables.Length > 1)
            {
                for (int i = 0; i < tables.Length - 1; i++)
                {
                    table = table.Get<LuaTable>(tables[i]);
                }
                key = tables[tables.Length - 1];
            }
            return table.Get<TValue>(key);
        }
    }
}
