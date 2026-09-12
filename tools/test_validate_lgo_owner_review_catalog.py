import unittest

import validate_lgo_owner_review_catalog as validator


class OwnerReviewCatalogValidatorTests(unittest.TestCase):
    def test_current_catalog_is_locked_to_vo(self):
        self.assertEqual(validator.main(), 0)


if __name__ == "__main__":
    unittest.main()
