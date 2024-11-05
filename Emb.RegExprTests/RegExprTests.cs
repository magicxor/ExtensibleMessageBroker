using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Emb.RegExprTests
{
    public class Tests
    {
        [Test]
        public void TestRegExps()
        {
            var testCaseCount = 0;
            var includeTests = 0;
            var excludeTests = 0;
            
            var testCasesDir = Path.Combine(TestContext.CurrentContext.TestDirectory, "Files");
            foreach (var testCaseDir in Directory.EnumerateDirectories(testCasesDir))
            {
                var includePatternsDir = Path.Combine(testCaseDir, "patterns", "include");
                var excludePatternsDir = Path.Combine(testCaseDir, "patterns", "exclude");
                var includeTestsDir = Path.Combine(testCaseDir, "tests", "include");
                var excludeTestsDir = Path.Combine(testCaseDir, "tests", "exclude");

                foreach (var testCaseFile in TestDir.EnumerateFiles(includeTestsDir))
                {
                    var testCase = File.ReadAllText(testCaseFile);

                    Assert.That(TestDir.EnumerateFiles(includePatternsDir).Any(patternFile => Regex.IsMatch(testCase, File.ReadAllText(patternFile))), Is.True, $"{nameof(testCaseDir)} = {testCaseDir}, {nameof(testCaseFile)} = {testCaseFile}, {nameof(testCase)} = {testCase}");
                    Assert.That(TestDir.EnumerateFiles(excludePatternsDir).Any(patternFile => Regex.IsMatch(testCase, File.ReadAllText(patternFile))), Is.False, $"{nameof(testCaseDir)} = {testCaseDir}, {nameof(testCaseFile)} = {testCaseFile}, {nameof(testCase)} = {testCase}");

                    includeTests++;
                }

                foreach (var testCaseFile in TestDir.EnumerateFiles(excludeTestsDir))
                {
                    var testCase = File.ReadAllText(testCaseFile);

                    Assert.That(
                        TestDir.EnumerateFiles(includePatternsDir).All(patternFile => !Regex.IsMatch(testCase, File.ReadAllText(patternFile)))
                        || TestDir.EnumerateFiles(excludePatternsDir).Any(patternFile => Regex.IsMatch(testCase, File.ReadAllText(patternFile))),
                        Is.True,
                        $"{nameof(testCaseDir)} = {testCaseDir}, {nameof(testCaseFile)} = {testCaseFile}, {nameof(testCase)} = {testCase}");

                    excludeTests++;
                }

                var model = new SerializableModel
                {
                    IncludedPatterns = TestDir.EnumerateFiles(includePatternsDir).Select(File.ReadAllText).ToList(),
                    ExcludedPatterns = TestDir.EnumerateFiles(excludePatternsDir).Select(File.ReadAllText).ToList(),
                };
                var serializedModel = JsonConvert.SerializeObject(model);
                var escapedSerializedModel = JsonConvert.ToString(serializedModel);
                TestContext.Out.WriteLine($"{Path.GetFileName(testCaseDir)} escaped model: {escapedSerializedModel}");
                
                testCaseCount++;
            }

            TestContext.Out.WriteLine($"{testCaseCount} test cases processed ({includeTests} {nameof(includeTests)}, {excludeTests} {nameof(excludeTests)})");
        }
    }
}
