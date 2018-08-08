using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivObservable.UnitTest
{
    class TestEventReceiver : IFixedEventReceiver<TestEventReceiver>
    {
        public int Id { get; set; }

        public SafeIterationList<TestEventReceiver> Children { get; } = new SafeIterationList<TestEventReceiver>();
        IEnumerable<IFixedEventReceiver<TestEventReceiver>> IFixedEventReceiver<TestEventReceiver>.Children => Children;

        public TestEventReceiver Receiver => this;

        public TestEventReceiver(int id)
        {
            Id = id;
        }

        public static (TestEventReceiver root, int count, List<TestEventReceiver> data)
            CreateData(int startId = 0, int width = 3, int depth = 3)
        {
            int id = startId;
            var data = new List<TestEventReceiver>();
            var root = new TestEventReceiver(id++);
            data.Add(root);

            if (depth > 0)
            {
                for (int i = 0; i < width; ++i)
                {
                    var (child, childCount, childData) = CreateData(id, width, depth - 1);
                    root.Children.Add(child);
                    id += childCount;
                    data.AddRange(childData);
                }
            }

            return (root, id - startId, data);
        }
    }
}
