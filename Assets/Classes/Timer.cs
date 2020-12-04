public class Timer
{
    float timer;
    float maxTimer;

    public Timer(float maxTimer)
    {
        this.timer = 0f;
        this.maxTimer = maxTimer;
    }

    public void Increment(float amount)
    {
        timer += amount;
    }

    public bool IsReached()
    {
        return timer >= maxTimer;
    }

    public void Reset()
    {
        timer = 0f;
    }
}