const isNullOrEmpty = (text: string) => {
	return text.length == 0 || text == null || text == undefined;
}

const filterBuilder = (baseUrl: string, json: any) => {
	if(baseUrl.charAt(baseUrl.length - 1) == '?') baseUrl = `${baseUrl}`

	let params = ''
	Object.entries(json).forEach((entry) => {
		const [key, value] = entry;
		if(typeof(value) == 'string' && !isNullOrEmpty(value)) {
			params = `${params}${key}=${value}&`
		}
		else if(Array.isArray(value)) {
			value.forEach(val => {
				params = `${params}${key}=${val}&`
			});
		}
		else if(value != null) {
			params = `${params}${key}=${value}&`
		}
	});

	if (params.length > 0) baseUrl = `${baseUrl}?${params}`

	return baseUrl;
}

export default filterBuilder;
