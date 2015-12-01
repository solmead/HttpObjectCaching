# HttpObjectCaching
A library to help simplify accessing of cached entries on a web site. This allows the user to treat Global / App Pool cache, Session Caching, and Request thread caching all the same. It is pluggable allowing that addition of a perminate and disconnected (i.e. redis type) cache as well. There is also a cookie type cache that is setup to store data in the perminate cache tied to a data key stored in a cookie.

