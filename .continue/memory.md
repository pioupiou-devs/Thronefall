# High-Level Architecture Summary

## Main Systems
- **GameManager**: Manages the overall game state and transitions.
- **PlayerController**: Handles player input and movement.
- **UISystem**: Manages all UI elements and interactions.
- **NetworkLayer**: Handles network communication and synchronization.
- **InputSystem**: Processes and provides input data to other systems.

## Key Relations
- `PlayerController depends_on InputSystem`

## Conventions
- Private fields use _camelCase.
- Serialized private fields use `[SerializeField] private int _speed;`.
- Statics use `s_StaticName`, Constants use `c_ConstantName`.
- Public properties/methods use PascalCase.