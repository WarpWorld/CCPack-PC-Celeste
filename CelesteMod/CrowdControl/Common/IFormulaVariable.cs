namespace CrowdControl.Common
{
    public interface IFormulaVariable
    {
        long Reduce();
        string FinalCode { get; }
        FormulaVariableType FormulaVariableType { get; }
    }

    public enum FormulaVariableType : byte
    {
        Effect = 0x00,
        EffectRequest = 0x01,
        InventoryItem = 0x02,
        RemoteItem = 0x03,

        FormulaBox = 0x10
    }
}
