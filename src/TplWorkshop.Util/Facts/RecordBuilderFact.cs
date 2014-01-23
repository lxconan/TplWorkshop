using System;
using TplWorkshop.Util.Visualizer;
using Xunit;
using Xunit.Extensions;

namespace TplWorkshop.Util.Facts
{
    public class RecordBuilderFact
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void should_set_to_untitled_if_name_is_null_or_whitespaced(string name)
        {
            var recordBuilder = CreateRecordBuilder();

            recordBuilder.Init(name);
            TaskRunningRecord record = recordBuilder.Build(null);

            Assert.Equal("Untitled 1", record.Name);
        }

        [Fact]
        public void should_not_call_init_twice()
        {
            RecordBuilder recordBuilder = CreateRecordBuilder();

            recordBuilder.Init(null);

            Assert.Throws<InvalidOperationException>(() => recordBuilder.Init(null));
        }

        [Fact]
        public void should_increase_untitled_count()
        {
            RecordBuilder.ResetTitleRef();
            RecordBuilder recordBuilder1 = CreateRecordBuilder(false);
            RecordBuilder recordBuilder2 = CreateRecordBuilder(false);

            recordBuilder1.Init(null);
            recordBuilder2.Init(null);

            Assert.Equal("Untitled 1", recordBuilder1.Build(null).Name);
            Assert.Equal("Untitled 2", recordBuilder2.Build(null).Name);
        }

        private static RecordBuilder CreateRecordBuilder(bool resetUntitleRefCount = true)
        {
            var recordBuilder = new RecordBuilder();
            if (resetUntitleRefCount)
            {
                RecordBuilder.ResetTitleRef();
            }


            return recordBuilder;
        }
    }
}