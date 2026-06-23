# Postman

Ready-to-use Postman collections for the Horoscope API.

## Files

| File | What it is |
|------|------------|
| `HoroscopeApi.postman_collection.json` | Our API: Auth, Profile, Horoscope and Stats endpoints. |
| `HoroscopeApi.postman_environment.json` | Environment with `baseUrl` and `token`. |
| `NewAstro.postman_collection.json` | The external horoscope provider consumed by the API (for reference/debugging). |

## How to use

1. In Postman, **Import** the three files (collections + environment).
2. Select the **Horoscope API - Local** environment (top-right).
3. Make sure the API is running. The default `baseUrl` is `http://localhost:5237` (HTTP profile). Change it in the environment if you use the HTTPS profile (`https://localhost:7194`).
4. Run **Auth → Register** (or **Auth → Login**). Its test script stores the returned JWT in the `token` variable automatically.
5. Run any protected request (Profile, Horoscope, Stats). They inherit `Authorization: Bearer {{token}}` from the collection, so no manual copy/paste is needed.

## Notes

- All API responses use the envelope `{ success, message, data, statusCode }`; the token is read from `data.token`.
- The sample user (`postman_user`) is independent from the seeded sample data, so you can register it on a fresh database without conflicts.
- `NewAstro` is an external dependency, not part of the deliverable. It is included only to document the upstream contract (and the chunked-encoding quirk handled in `NewAstroClient`).
