#!/bin/bash
PACKAGE_DIR="$1"
CONFIGURATION="$2"
PLATFORM="$3"
ARCHITECTURE="$4"
OUTPUT_FILE="$5"

# set other variables
BINARY_DIR="bin/$CONFIGURATION-$PLATFORM-$ARCHITECTURE"
COPY_COMMAND="cp -rf"
MKDIR_COMMAND="mkdir -p"

# create directories
$MKDIR_COMMAND $PACKAGE_DIR

# copy binaries
echo "Copying binaries..."
$COPY_COMMAND "$BINARY_DIR/host" "$PACKAGE_DIR"
$COPY_COMMAND "$BINARY_DIR/SchemaGenerator" "$PACKAGE_DIR"
for f in $BINARY_DIR/host/System*.dll; do
    cp -rf "$f" "$PACKAGE_DIR/SchemaGenerator"
done
#$COPY_COMMAND "$BINARY_DIR/MapDesigner" "$PACKAGE_DIR"
#if [[ "$PLATFORM" = "windows" ]]; then
#    $COPY_COMMAND "$BINARY_DIR/MapDesigner-Internals/MapDesigner-Internals.dll" "$PACKAGE_DIR/MapDesigner"
#fi

# compress into a tarball
echo "Compressing..."
tar -cf $OUTPUT_FILE $PACKAGE_DIR

# delete intermediate directory
echo "Deleting intermediate directory..."
rm -rf $PACKAGE_DIR

echo "Done! Output written to $OUTPUT_FILE"