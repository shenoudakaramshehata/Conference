

namespace Conference
{
    public class SharedResource
    {

        int[] arr = { 4, 4 };
        public SharedResource()
        {
            x();
        }
        public void x()
        {
            var v = arr.Clone();
            var x = v;
        }

    }
}
