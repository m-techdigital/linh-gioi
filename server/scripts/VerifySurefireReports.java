import java.nio.file.Files;
import java.nio.file.Path;
import java.util.ArrayList;
import java.util.Comparator;
import java.util.List;
import javax.xml.XMLConstants;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.Element;

/**
 * Postcondition for server build/test wrappers: a successful Maven exit is not enough.
 * Fresh Surefire reports must exist and at least one non-skipped test must have executed.
 */
public final class VerifySurefireReports {
    private VerifySurefireReports() {}

    public static void main(String[] args) throws Exception {
        if (args.length == 0) {
            fail("no Surefire report directories were supplied");
        }

        List<Path> reports = new ArrayList<>();
        for (String argument : args) {
            Path directory = Path.of(argument);
            if (!Files.isDirectory(directory)) {
                continue;
            }
            try (var files = Files.list(directory)) {
                files.filter(path -> path.getFileName().toString().startsWith("TEST-"))
                        .filter(path -> path.getFileName().toString().endsWith(".xml"))
                        .forEach(reports::add);
            }
        }
        reports.sort(Comparator.naturalOrder());

        if (reports.isEmpty()) {
            fail("no fresh Surefire TEST-*.xml reports were found");
        }

        var factory = DocumentBuilderFactory.newInstance();
        factory.setFeature("http://apache.org/xml/features/disallow-doctype-decl", true);
        factory.setFeature("http://xml.org/sax/features/external-general-entities", false);
        factory.setFeature("http://xml.org/sax/features/external-parameter-entities", false);
        factory.setAttribute(XMLConstants.ACCESS_EXTERNAL_DTD, "");
        factory.setAttribute(XMLConstants.ACCESS_EXTERNAL_SCHEMA, "");

        long declared = 0;
        long skipped = 0;
        long failures = 0;
        long errors = 0;
        for (Path report : reports) {
            Element suite = factory.newDocumentBuilder().parse(report.toFile()).getDocumentElement();
            declared += longAttribute(suite, "tests", report);
            skipped += longAttribute(suite, "skipped", report);
            failures += longAttribute(suite, "failures", report);
            errors += longAttribute(suite, "errors", report);
        }

        long executed = declared - skipped;
        if (declared < 0 || skipped < 0 || executed <= 0) {
            fail("executed test count is zero (declared=" + declared + ", skipped=" + skipped + ")");
        }
        if (failures != 0 || errors != 0) {
            fail("Surefire reports contain failures/errors (failures=" + failures + ", errors=" + errors + ")");
        }

        System.out.printf(
                "TEST_EVIDENCE_PASS executed=%d declared=%d skipped=%d reports=%d%n",
                executed, declared, skipped, reports.size());
    }

    private static long longAttribute(Element suite, String name, Path report) {
        String value = suite.getAttribute(name);
        if (value == null || value.isBlank()) {
            fail("missing '" + name + "' attribute in " + report);
        }
        try {
            return Long.parseLong(value);
        } catch (NumberFormatException exception) {
            fail("invalid '" + name + "' attribute in " + report + ": " + value);
            return -1;
        }
    }

    private static void fail(String message) {
        System.err.println("ERROR: " + message);
        System.exit(8);
    }
}
