namespace SmartWaste.Model.JsonModels.Unit
{
    public class BinLockInput
    {
        public int UnitId { get; set; }
        public bool BinLock { get; set; }
        public DateTime? LockUntil { get; set; }
    }
}
