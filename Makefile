site-output:
	@echo "Building site..."
	@jekyll build -d $@
clean:
	@echo "Deleting output directories"
	@rm -rf "site-output"