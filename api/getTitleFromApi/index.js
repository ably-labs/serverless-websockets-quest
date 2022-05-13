// eslint-disable-next-line no-unused-vars
module.exports = async function (context, req) {

    const responseJson = { title: "This is a title from the api call!" };
    context.res = {
        // status: 200, /* Defaults to 200 */
        body: responseJson,
        contentType: 'application/json'
    };
}
