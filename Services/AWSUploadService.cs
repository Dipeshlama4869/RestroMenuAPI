using Amazon.S3;

namespace RestroMenu.Services
{
    public class AWSUploadService
    {
        private string bucketName = "imssoftware-cdn-images";
        private RegionEndpoint bucketRegion = RegionEndpoint.APSoutheast1;


        private string KeyName = "ims.png";

        private string accesskey = "AKIAQOZMEZEAFXD7USPE";
        private string secretkey = "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";
        private IAmazonS3 s3Client;

        public AWSUploadService(IConfiguration configuration)
        {
            var bkName = configuration.GetSection("bucketname").Value;
            if (!string.IsNullOrEmpty(bkName)) bucketName = bkName.ToString();
            var acckey = configuration.GetSection("awsaccesskey").Value;
            if (!string.IsNullOrEmpty(acckey)) accesskey = acckey;
            var secret = configuration.GetSection("aswsecretkey").Value;
            if (!string.IsNullOrEmpty(secret)) secretkey = secret;
        }

        public void UploadToAWS(string keyname, string filePath)
        {
            AWSCredentials credentials = new Amazon.Runtime.BasicAWSCredentials(accesskey, secretkey);

            s3Client = new AmazonS3Client(credentials, bucketRegion);

            try
            {
                var fileTransferUtility = new TransferUtility(s3Client);
                fileTransferUtility.Upload(filePath, bucketName);
                //Put object-specify only key name for the new object
                //        var putRequest1 = new TransferUtilityUploadRequest
                //        {
                //            BucketName = bucketName,
                //            Key = KeyName,
                //            ContentType="image"
                //        };
                //        |
                //PutObjectResponse response1 = s3Client.PutObjectAsync(putRequest1);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                    "Error encountered on server. Message:'{0}' when writing an object", e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                     , e.Message);
            }
        }

        public async Task<bool> DeleteInAws(string keyname)
        {
            AWSCredentials credentials = new Amazon.Runtime.BasicAWSCredentials(accesskey, secretkey);

            s3Client = new AmazonS3Client(credentials, bucketRegion);
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyname
                };
                await s3Client.DeleteObjectAsync(deleteRequest);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Exists(string keyname)
        {
            AWSCredentials credentials = new Amazon.Runtime.BasicAWSCredentials(accesskey, secretkey);

            s3Client = new AmazonS3Client(credentials, bucketRegion);
            try
            {
                var response = await s3Client.GetObjectMetadataAsync(new GetObjectMetadataRequest()
                { BucketName = bucketName, Key = keyname });

                return true;
            }

            catch (Amazon.S3.AmazonS3Exception ex)
            {
                if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;

                //status wasn't not found, so throw the exception
                throw;
            }
        }
    }
}
