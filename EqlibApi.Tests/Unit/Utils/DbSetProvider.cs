using EqlibApi.Models.Db;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace EqlibApi.Tests.Unit.Utils
{
    class DbSetProvider
    {
        public static Mock<DbSet<T>> MockSet<T>(List<T> list) where T : class
        {
            var source = list.AsQueryable();
            var set = new Mock<DbSet<T>>();
            set.As<IAsyncEnumerable<T>>().Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(source.GetEnumerator()));

            set.As<IQueryable<T>>().Setup(x => x.Provider).Returns(new TestAsyncQueryProvider<T>(source.Provider));
            set.As<IQueryable<T>>().Setup(x => x.Expression).Returns(source.Expression);
            set.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(source.ElementType);
            set.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(source.GetEnumerator());

            set.Setup(s => s.AddAsync(It.IsAny<T>(), It.IsAny<CancellationToken>()))
                .Callback((T e, CancellationToken t) => list.Add(e))
                .ReturnsAsync((T c, CancellationToken t) => null);
            return set;
        }
        public static DbSet<T> CreateSet<T>(List<T> list) where T : class
        {
            var set = MockSet(list);
            return set.Object;
        }
    }
}
