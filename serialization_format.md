# HyperLogLog Serialization Format (HLL.NET)

This document describes the binary format used for serializing `HyperLogLog<T>` instances.

## Format Structure

The serialized data is a byte array with the following layout:

| Offset | Size     | Description                                      |
|--------|----------|--------------------------------------------------|
| 0      | 1 byte   | Format version (currently `2`)                   |
| 1      | 1 byte   | Type name length (`L`)                           |
| 2      | 1 byte   | Precision value (`p`, between 4–16)              |
| 3      | `L`      | UTF-8 encoded full type name (`typeof(T).FullName`) |
| 3+L    | N bytes  | HLL registers (one byte per register; length = `2^p`) |

### Example

For a `HyperLogLog<string>` with precision 14:

- Format version = `2`
- Type name = `System.String`, which has length = `13`
- Registers count = `2^14 = 16384`
- Total bytes = `1 + 1 + 1 + 13 + 16384 = 16400 bytes`

## Versioning

- Current format version: `2`
- Future versions must increment the first byte and adjust parsing logic accordingly.

## Notes

- If the deserialized type `T` does not match the stored type name, deserialization will fail.
- Type names are used to avoid logic errors when deserializing to the wrong generic type.
