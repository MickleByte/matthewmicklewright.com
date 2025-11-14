# Deployment Guide

This project is configured to automatically deploy to AWS S3 when code is pushed to the `main` branch.

## Prerequisites

1. An AWS account with an S3 bucket configured for static website hosting
2. AWS credentials with permissions to:
   - Upload files to S3
   - Configure S3 bucket website settings
   - (Optional) Invalidate CloudFront distributions

## GitHub Secrets Setup

Configure the following secrets in your GitHub repository settings (Settings → Secrets and variables → Actions):

### Required Secrets

- `AWS_ACCESS_KEY_ID` - Your AWS access key ID
- `AWS_SECRET_ACCESS_KEY` - Your AWS secret access key
- `S3_BUCKET_NAME` - The name of your S3 bucket (e.g., `matthewmicklewright.com`)

### Optional Secrets

- `AWS_REGION` - AWS region (defaults to `us-east-1` if not set)
- `CLOUDFRONT_DISTRIBUTION_ID` - CloudFront distribution ID if using CloudFront (leave empty if not using)

## S3 Bucket Configuration

### 1. Enable Static Website Hosting

In your S3 bucket settings:
- Go to **Properties** → **Static website hosting**
- Enable static website hosting
- Set **Index document** to: `index.html`
- Set **Error document** to: `index.html` (required for React Router)

### 2. Bucket Policy

Add a bucket policy to allow public read access:

```json
{
  "Version": "2012-10-17",
  "Statement": [
    {
      "Sid": "PublicReadGetObject",
      "Effect": "Allow",
      "Principal": "*",
      "Action": "s3:GetObject",
      "Resource": "arn:aws:s3:::YOUR-BUCKET-NAME/*"
    }
  ]
}
```

### 3. Block Public Access Settings

- Uncheck **Block all public access** (or configure specific settings)
- This is required for static website hosting

## CloudFront (Optional but Recommended)

For better performance and HTTPS support:

1. Create a CloudFront distribution pointing to your S3 bucket
2. Set the **Default Root Object** to `index.html`
3. Add a custom error response:
   - **HTTP Error Code**: 403
   - **Response Page Path**: `/index.html`
   - **HTTP Response Code**: 200
4. Add another custom error response:
   - **HTTP Error Code**: 404
   - **Response Page Path**: `/index.html`
   - **HTTP Response Code**: 200
5. Add the CloudFront distribution ID to GitHub secrets as `CLOUDFRONT_DISTRIBUTION_ID`

## Manual Deployment

You can also trigger the deployment manually:

1. Go to **Actions** tab in GitHub
2. Select **Deploy to S3** workflow
3. Click **Run workflow**

## How It Works

1. On push to `main`, the workflow:
   - Checks out the code
   - Installs dependencies
   - Builds the React app (outputs to `dist/`)
   - Syncs files to S3 with appropriate cache headers
   - Configures S3 website settings
   - Invalidates CloudFront cache (if configured)

2. Static assets (JS, CSS, images) are cached for 1 year
3. HTML files are set to no-cache to ensure fresh content

## Troubleshooting

- **403 Forbidden**: Check bucket policy and public access settings
- **404 on routes**: Ensure error document is set to `index.html` in S3
- **Build fails**: Check Node.js version compatibility
- **Deployment fails**: Verify AWS credentials have correct permissions

