# Currency Converter API

A high-performance currency conversion API built with ASP.NET Core, Redis, ELK, and external exchange rate providers.

---

## 🚀 Setup Instructions

### Prerequisites

- [Docker](https://www.docker.com/)

1. Clone the repository:

   ```bash
   git clone https://github.com/your-org/currency-converter.git
   cd currency-converter
   ```
2. Start app
   ```bash
   make currency
   ```
3. Close app
   ```bash
   make stop
   ```

### Assumptions Made
- 🔁 **Frankfurter API** is always available and responds within acceptable latency bounds. \
  The system uses retry and circuit breaker policies, but it's assumed downtime is rare and short-lived.

- 💰 **Exchange rates** do not change more frequently than once per day. \
  Caching is based on this assumption to reduce API pressure.  

- ⛔ **Currencies** TRY, PLN, THB, and MXN are considered restricted for business or compliance reasons. \
  Any request involving these currencies is treated as invalid.

- 📅 **Historical exchange rates** are only required from the Frankfurter API's supported range. \
  No data transformations or corrections are applied for missing or partial data.

- 🌐 **All operations are performed in UTC**. \
  The system assumes clients either use UTC or convert to/from it on their side.

- 🧪 **JWT tokens** are trusted and already validated before reaching the endpoint logic. \
Token validation and RBAC are assumed to be handled at middleware level.

- 📦 **The caching layer** (e.g., Redis) is operational and used consistently across requests. \
If the cache fails, the fallback is to re-query the external provider.

- 🔐 **Rate limiting** is configured per client (e.g., by ClientId in JWT) and not globally. \
This assumes fair usage distribution among multiple consumers.

- 🔎 **Pagination for historical data** assumes valid date ranges and sane page sizes. \
No internal guardrails beyond basic validation are in place for absurd requests (e.g., 50 years of data in one go).

- 🏗️ **Deployment environments** (Dev, Test, Prod) have consistent configurations apart from secrets and URLs.