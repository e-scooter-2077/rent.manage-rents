# Rent/Manage Rents
[![Continuous Integration](https://github.com/e-scooter-2077/rent.manage-rents/actions/workflows/ci.yml/badge.svg?event=push)](https://github.com/e-scooter-2077/rent.manage-rents/actions/workflows/ci.yml)
[![GitHub issues](https://img.shields.io/github/issues-raw/e-scooter-2077/rent.manage-rents?style=plastic)](https://github.com/e-scooter-2077/rent.manage-rents/issues)
[![GitHub pull requests](https://img.shields.io/github/issues-pr-raw/e-scooter-2077/rent.manage-rents?style=plastic)](https://github.com/e-scooter-2077/rent.manage-rents/pulls)
[![GitHub](https://img.shields.io/github/license/e-scooter-2077/rent.manage-rents?style=plastic)](/LICENSE)
[![GitHub release (latest SemVer including pre-releases)](https://img.shields.io/github/v/release/e-scooter-2077/rent.manage-rents?include_prereleases&style=plastic)](https://github.com/e-scooter-2077/rent.manage-rents/releases)
[![Documentation](https://img.shields.io/badge/documentation-click%20here-informational?style=plastic)](https://e-scooter-2077.github.io/documentation/implementation/index.html#event-handling)

This is a Service Bus triggered Azure Function that reacts to the events of the Rent Microservice signaling the confirmation and stop (either natural or forced) of a rent from a customer on a scooter and create the corresponding relationship on the Digital Twin Graph.

