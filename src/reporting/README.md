# Report Catalog React App

This project is a React application integrated into a .NET 4.6.1 MVC application, located in the `~Content/report-catalog` folder. The build output with static files is located in `~Content/report-catalog/build/static`.

## Project Setup

The React application is loaded through the `ReportCatalogController` and the associated `Index.cshtml` view. To make changes to the React application and reflect those changes in the MVC application, follow these steps.

## Available Scripts

In the project directory, you can run:

### `npm start`

Runs the app in the development mode. Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

The page will reload if you make edits. You will also see any lint errors in the console.

### `npm test`

Launches the test runner in the interactive watch mode. See the section about running tests for more information.

### `npm run build`

Builds the app for production to the `build` folder. It correctly bundles React in production mode and optimizes the build for the best performance.

The build is minified, but the filenames do not include the hashes. Your app is ready to be deployed!

After running the build, the static files will be located in the `~Content/report-catalog/build/static` directory.

## Deploying Changes

To deploy changes to the MVC application:

1. Run `npm run build` to generate the updated build files.
2. Update the path to the main JavaScript file in the `Index.cshtml` view.

###  `Views/ReportCatalog/Index.cshtml`


```
@{     
	ViewBag.Title = "Report Catalog";     
	Layout = "~/Views/Shared/_LayoutNew.cshtml"; 
}  
<script src="~/Content/report-catalog/build/static/js/main.js"></script>  
<div id="root"></div>
```

## Additional Information

### `npm run eject`

**Note: this is a one-way operation. Once you eject, you can’t go back!**

If you aren’t satisfied with the build tool and configuration choices, you can eject at any time. This command will remove the single build dependency from your project.

Instead, it will copy all the configuration files and the transitive dependencies (webpack, Babel, ESLint, etc.) right into your project so you have full control over them. All of the commands except eject will still work, but they will point to the copied scripts so you can tweak them. At this point, you’re on your own.

You don’t have to ever use eject. The curated feature set is suitable for small and middle deployments, and you shouldn’t feel obligated to use this feature. However, we understand that this tool wouldn’t be useful if you couldn’t customize it when you are ready for it.

## Learn More

You can learn more in the Create React App documentation.

To learn React, check out the [React documentation](https://reactjs.org/).