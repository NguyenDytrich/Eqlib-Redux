using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;

namespace EqlibApi.Tests.Unit.Utils
{
    class MockDbContext
    {
        public static DbSet<T> CreateSet<T>(IEnumerable<T> enumerable) where T : class
        {
            var source = enumerable.AsQueryable();
            var set = new Mock<DbSet<T>>();
            set.As<IAsyncEnumerable<T>>().Setup(x => x.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(source.GetEnumerator()));

            set.As<IQueryable<T>>().Setup(x => x.Provider).Returns(new TestAsyncQueryProvider<T>(source.Provider));
            set.As<IQueryable<T>>().Setup(x => x.Expression).Returns(source.Expression);
            set.As<IQueryable<T>>().Setup(x => x.ElementType).Returns(source.ElementType);
            set.As<IQueryable<T>>().Setup(x => x.GetEnumerator()).Returns(source.GetEnumerator());
            return set.Object;
        }
    }
}
