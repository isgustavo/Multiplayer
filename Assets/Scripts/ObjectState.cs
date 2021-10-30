public class ObjectState
{
    public uint ObjectTick { get; set; }
    public ObjectTickState[] ObjectStateBuffer { get; private set; } = new ObjectTickState[1024];

    public ObjectState ()
    {
        for (int i = 0; i < 1024; i++)
        {
            ObjectStateBuffer[i] = new ObjectTickState();
        }
    }

    public void ReceivedTick(uint tick)
    {
        ObjectTick = tick;
    }

    public void Tick()
    {
        ObjectTick++;
    }
}
